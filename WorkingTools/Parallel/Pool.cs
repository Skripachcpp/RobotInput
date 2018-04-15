using System;

namespace WorkingTools.Parallel
{
    public class Pool : PoolLite
    {
        public Pool()
            : base()
        {
        }

        public Pool(int parallel)
            : base(parallel)
        {
        }

        public new void Add(ICallback callback)
        {
            base.Add(callback);
        }

        public void Add<T>(Action<T> action, T p1)
        {
            if (action == null) return;
            Add(new Callback<T>(action, p1));
        }

        public void Add<T1, T2>(Action<T1, T2> action, T1 p1, T2 p2)
        {
            if (action == null) return;
            Add(new Callback<T1, T2>(action, p1, p2));
        }

        public void Add<T1, T2, T3>(Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3)
        {
            if (action == null) return;
            Add(new Callback<T1, T2, T3>(action, p1, p2, p3));
        }

        public void Add<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            if (action == null) return;
            Add(new Callback<T1, T2, T3, T4>(action, p1, p2, p3, p4));
        }

        public void Add<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            if (action == null) return;
            Add(new Callback<T1, T2, T3, T4, T5>(action, p1, p2, p3, p4, p5));
        }

        public void Invoke(ICallback callback)
        {
            Add(callback);
            Invoke();
        }

        public void Invoke(Action action)
        {
            Add(action);
            Invoke();
        }

        public void Invoke<T>(Action<T> action, T state)
        {
            Add(action, state);
            Invoke();
        }

        public void Invoke<T1, T2>(Action<T1, T2> action, T1 p1, T2 p2)
        {
            Add(action, p1, p2);
            Invoke();
        }

        public void Invoke<T1, T2, T3>(Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3)
        {
            Add(action, p1, p2, p3);
            Invoke();
        }

        public void Invoke<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            Add(action, p1, p2, p3, p4);
            Invoke();
        }

        public void Invoke<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            Add(action, p1, p2, p3, p4, p5);
            Invoke();
        }


        /// <summary>
        /// Ждать завершения всех задач
        /// </summary>
        /// <param name="millisecondsTimeout">максимальный период ожидания</param>
        /// <returns>true если задачи завершились до окончиния периода ожидания</returns>
        public bool WaitAll(int millisecondsTimeout)
        {
            return IsComplit.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Ждать пока все задачи не будут запущены
        /// </summary>
        /// <param name="millisecondsTimeout">максимальный период ожидания</param>
        /// <returns>true если задачи завершились до окончиния периода ожидания</returns>
        public bool WaitAllRunning(int millisecondsTimeout)
        {
            return QueueIsEmptyOrNotProcess.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Ждать завершения любой задачи
        /// </summary>
        /// <param name="millisecondsTimeout">максимальный период ожидания</param>
        /// <returns>true если задачи завершились до окончиния периода ожидания</returns>
        public bool WaitAny(int millisecondsTimeout)
        {
            ResetAnyComplete();
            return AnyComplete.WaitOne(millisecondsTimeout);
        }

        public virtual bool Dispose(bool wait, int millisecondsTimeout)
        {
            Clear();
            return !wait || WaitAll(millisecondsTimeout);
        }
    }
}
