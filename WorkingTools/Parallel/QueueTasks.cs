using System;
using System.Collections.Generic;
using System.Linq;
using WorkingTools.Repository;

namespace WorkingTools.Parallel
{
    /// <summary>
    /// Асинхронная очередь задач
    /// </summary>
    /// <typeparam name="TActionParams">тип параметра передаваемого в action при выполнении очереди; 
    /// данный тип обязательно должен поддерживать стандартную сериализацию</typeparam>
    /// <remarks>очередь задач сохраняется в репозиторий и загружается от туда, 
    /// что позволяет продолжить выполение после перезапуска приложеиня;
    /// задачи завершившиеся ошибкой будут повторно обработаны после вызова метода ReRunFailTasks</remarks>
    public class QueueTasks<TActionParams> : IDisposable
    {
        private readonly object _lock = new object();

        private readonly Pool _pool;
        private readonly Action<TActionParams> _action;
        private readonly Action<TActionParams, Exception> _wasError;//если произошла ошибка в action
        private readonly IRepository<int, TActionParams> _repository;
        private readonly HashSet<int> _failTasks = new HashSet<int>();

        private int _maxKey;
        private bool _queueLoading;
        private bool _saveAuto;

        public QueueTasks(Action<TActionParams> action, IRepository<int, TActionParams> repository, Action<TActionParams, Exception> wasError = null)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (repository == null) throw new ArgumentNullException("repository");

            _pool = new Pool();
            _parallel = _pool.Parallel;
            _saveAuto = true;

            _action = action;
            _wasError = wasError;
            _repository = repository;
        }


        private int _parallel;
        /// <summary>
        /// Допустимое число задач выполняемых паралельно
        /// </summary>
        /// <remarks>вы можете выставить кол-во паралельно выполняемых задач в 0,
        /// активные задачи будут завершены, а ожидающие своей очереди задачи не будут запущены,
        /// обработка задач будет считаться завершенной и не нечнется до вызова метода Invoke();
        /// если вновь изменить свойство на отличное от 0 до того того как все активные задачи были завершены,
        /// кол-во паралельно выполняемых задач будут стремиться к указаному</remarks>
        public int Parallel { get { return _parallel; } set { lock (_lock) _parallel = _pool.Parallel = value; } }

        /// <summary>
        /// Выполнение задачи в очереди
        /// </summary>
        /// <param name="prms">параметры</param>
        private static void Perform(PerformParams prms)
        {
            try
            {
                prms.Action(prms.ActionParams);

                prms.Repository.Remove(prms.KeyItem);
                if (prms.SaveAuto) prms.Repository.Save();
            }
            catch (Exception ex)
            {
                prms.FailTasks.Add(prms.KeyItem);

                if (prms.WasError != null)
                    try { prms.WasError(prms.ActionParams, ex); }
                    catch (Exception){/*ignored*/}
            }
        }

        /// <summary>
        /// Запустить повторную обработку задач завершившихся ошибкой
        /// </summary>
        /// <returns>true если запущена обработка; false если задач завершившихся ошибкой не найдено</returns>
        public bool ReRunFailTasks()
        {
            bool startAny = false;
            foreach (var key in _failTasks)
            {
                TActionParams value;
                if (_repository.Get(key, out value))
                {
                    _pool.Invoke(Perform, new PerformParams(_repository, key, _action, _wasError, value, _saveAuto, _failTasks));
                    startAny = true;
                }
            }

            _failTasks.Clear();
            
            if (startAny) 
                _pool.Invoke();    

            return startAny;
        }

        /// <summary>
        /// Запустить обработку очереди
        /// </summary>
        /// <remarks>при первом вызове происходит подгрузка задач из репозитория</remarks>
        public void Start()
        {
            _pool.Parallel = Parallel;//значение _pool.Parallel меняется в Stop() и может отличатся от Parallel

            lock (_lock)
            {
                if (!_queueLoading)
                {
                    if (!_queueLoading)
                    {
                        foreach (var pair in _repository.Get().OrderBy(pair => pair.Key))
                        {
                            if (_maxKey < pair.Key) _maxKey = pair.Key; //искать максимальное значение

                            _pool.Invoke(Perform, new PerformParams(_repository, pair.Key, _action, _wasError, pair.Value, _saveAuto, _failTasks));
                        }

                        _queueLoading = true;
                    }
                }
                else 
                {
                    if (!_pool.IsRunning)
                        _pool.Invoke();
                }
            }
        }

        /// <summary>
        /// Добавить задачу в очередь
        /// </summary>
        /// <param name="actionParams"></param>
        /// <remarks>при добавлении новой задачи очередь автоматически начинает выполняться, 
        /// при этом первыми будут выполнены ранее сохраненные задачи</remarks>
        public void Enqueue(TActionParams actionParams)
        {
            Start();

            lock (_lock)
            {
                _maxKey++;

                _repository.Set(_maxKey, actionParams);
                if (_saveAuto) Save();

                _pool.Invoke(Perform, new PerformParams(_repository, _maxKey, _action, _wasError, actionParams, _saveAuto, _failTasks));
            }
        }

        /// <summary>
        /// Преостановить выполнение задач
        /// </summary>
        public void Stop()
        {
            lock (_lock)
            {
                _pool.Parallel = 0;

                _pool.WaitAll();
            }
        }

        /// <summary>
        /// Очистить очередь задач
        /// </summary>
        public void Clear(bool wait = true)
        {
            lock (_lock)
            {
                _repository.Clear();
                _pool.Clear();

                if (wait) _pool.WaitAll();
            }
        }

        /// <summary>
        /// Сохранить состояние очереди
        /// </summary>
        public void Save()
        {
            _repository.Save();
        }

        /// <summary>
        /// Установка параметра автоматического сохранения изменений
        /// </summary>
        /// <param name="saveAuto">true если необходимо сохранять значение в репозитории после каждого изменения; 
        /// false изменения сохраняются только при вызове метода Save(), или при изменении параметра с false на true</param>
        public void SaveAuto(bool saveAuto)
        {
            if (!_saveAuto && saveAuto)
                Save();

            _saveAuto = saveAuto;
        }

        /// <summary>
        /// Ждать завершения всех задач
        /// </summary>
        public void WaitAll()
        {
            _pool.WaitAll();
        }
        
        /// <summary>
        /// Ждать завершения всех задач
        /// </summary>
        /// <param name="millisecondsTimeout">максимальный период ожидания</param>
        /// <returns>true если задачи завершились до окончиния периода ожидания</returns>
        public bool WaitAll(int millisecondsTimeout)
        {
            return _pool.WaitAll(millisecondsTimeout);
        }

        #region events
        
        public event EventHandler Disposed;

        protected virtual void OnDisposed()
        {
            var handler = Disposed;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion events

        public virtual void Dispose()
        {
            OnDisposed();

            lock (_lock)
            {
                if (_pool != null) _pool.Dispose();//критично уничтожить pool до repository
                if (_repository != null) _repository.Dispose();
            }
        }

        #region classes

        private class PerformParams
        {
            public PerformParams(IRepository<int, TActionParams> repository, int keyItem,
            Action<TActionParams> action, Action<TActionParams, Exception> wasError, TActionParams actionParams,
            bool saveAuto, HashSet<int> failTasks)
            {
                Repository = repository;
                KeyItem = keyItem;
                Action = action;
                ActionParams = actionParams;
                SaveAuto = saveAuto;
                FailTasks = failTasks;
                WasError = wasError;
            }

            /// <summary>
            /// репозиторий задач; при успешном выполнении задача удаляется из репозитория
            /// </summary>
            public IRepository<int, TActionParams> Repository { get; set; }

            /// <summary>
            /// идентификатор задачи в репозитории
            /// </summary>
            public int KeyItem { get; set; }
            /// <summary>
            /// обработчик задачи
            /// </summary>
            public Action<TActionParams> Action { get; set; }

            /// <summary>
            /// Обрабосчик вызываемый в случае возникновения ошибки в Action
            /// </summary>
            public Action<TActionParams, Exception> WasError { get; set; }

            /// <summary>
            /// параметры передаваемые обработчику
            /// </summary>
            public TActionParams ActionParams { get; set; }
            /// <summary>
            /// true если необходимо сохранять изменения после изменения репозитория
            /// </summary>
            public bool SaveAuto { get; set; }
            /// <summary>
            /// список задач завершившихся ошибкой
            /// </summary>
            /// <remarks>задача добавляет туда свой ключь, если обработка завершилась ошибкой</remarks>
            public HashSet<int> FailTasks { get; set; }
        }

        #endregion classes
    }
}