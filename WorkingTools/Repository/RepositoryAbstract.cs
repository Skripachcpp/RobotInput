using System;
using System.Collections.Generic;
using WorkingTools.Extensions;

namespace WorkingTools.Repository
{
    /// <summary>
    /// Репозиторий "один ключ - одно значение" с отложенным сохранением и загрузкой по требованию
    /// </summary>
    /// <typeparam name="TKey">ключ (обязательна перегрузка GetHashCode и Equals)</typeparam>
    /// <typeparam name="TValue">элемент (желательна перегрузка GetHashCode и Equals)</typeparam>
    public abstract class RepositoryAbstract<TKey, TValue> : IRepository<TKey, TValue>, IDisposable
    {
        protected object Lock = new object();

        private bool _loaded = false;

        private readonly Dictionary<TKey, TValue> _items = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TKey, TValue> _newItems = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TKey, TValue> _delItems = new Dictionary<TKey, TValue>();
        private readonly Dictionary<TKey, TValue> _updItems = new Dictionary<TKey, TValue>();

        public virtual bool Contains(TKey key)
        {
            lock (Lock)
            {
                if (_loaded == false) Load();

                return _items.ContainsKey(key);
            }
        }

        public virtual IEnumerable<KeyValuePair<TKey, TValue>> Get()
        {
            lock (Lock)
            {
                if (_loaded == false) Load();

                return _items;
            }
        }

        public virtual bool Get(TKey key, out TValue value)
        {
            lock (Lock)
            {
                if (_loaded == false) Load();

                if (_items.ContainsKey(key))
                {
                    value = _items[key];
                    return true;
                }
                else
                {
                    value = default(TValue);
                    return false;
                }
            }
        }

        public virtual TValue Get(TKey key)
        {
            TValue value;
            Get(key, out value);
            return value;
        }

        public virtual void Set(TKey key, TValue value)
        {
            lock (Lock)
            {
                if (_loaded == false) Load();

                /*если элемент существует*/
                if (_items.ContainsKey(key))
                {
                    /*если это новый элемент то изменитьего значение в справочнике измененных*/
                    if (_newItems.ContainsKey(key))
                        _newItems[key] = value;
                    else
                        _updItems.Add(key, value);


                    _items[key] = value;
                }
                /*если элемент не существует*/
                else
                {
                    /*если элемент есть в списке удаленных*/
                    if (_delItems.ContainsKey(key))
                    {
                        /*если его занчение не совпадает с добавляемым, то добавить в список измененных*/
                        if (!Equals(_delItems[key], value))
                            _updItems.Add(key, value);

                        _delItems.Remove(key);
                    }
                    else
                    {
                        _newItems.Add(key, value);
                    }


                    _items.Add(key, value);
                }
            }
        }

        public virtual bool Remove(TKey key)
        {
            lock (Lock)
            {
                if (_loaded == false) Load();

                if (!_items.ContainsKey(key))
                    return false;

                /*если удаляется не новый элемент (сохраненный) то добавить его в список удаленных*/
                if (!_newItems.Remove(key))
                    _delItems.Add(key, _items[key]);
                /*иначе удалить его из измененных (если он там есть)*/
                else
                    _updItems.Remove(key);

                /*и удалить его из основного списка*/
                return _items.Remove(key);
            }
        }

        public virtual void Clear()
        {
            lock (Lock)
            {
                if (_loaded == false) Load();

                /*очистить список новых элементов*/
                _newItems.ForEach(item => _items.Remove(item.Key));
                _newItems.Clear();

                /*очистить список измененных элементов*/
                _updItems.Clear();

                /*перенести все оставшиеся элементы в удаленные*/
                _items.ForEach(item => _delItems.Add(item.Key, item.Value));
                _items.Clear();
            }
        }

        public virtual void Load()
        {
            lock (Lock)
            {
                _items.Clear();

                _newItems.Clear();
                _delItems.Clear();
                _updItems.Clear();

                /*добавить загруженные элементы в _items*/
                LoadInems().Do(items => items.ForEach(item => _items.Add(item.Key, item.Value)));

                _loaded = true;
            }
        }

        public virtual void Save()
        {
            lock (Lock)
            {
                /*если история не загружена*/
                if (_loaded == false)
                    return;

                /*если в истории нет изменений*/
                if (_newItems.Count <= 0 && _delItems.Count <= 0)
                    return;

                SaveItems(_items, _newItems, _updItems, _delItems);

                _newItems.Clear();
                _delItems.Clear();
                _updItems.Clear();
            }
        }

        public void Dispose()
        {
            lock (Lock)
            {
                _items.Clear();
                _newItems.Clear();
                _delItems.Clear();
                _updItems.Clear();
            }
        }

        protected abstract IEnumerable<Pair<TKey, TValue>> LoadInems();

        protected abstract void SaveItems(IEnumerable<KeyValuePair<TKey, TValue>> items, IEnumerable<KeyValuePair<TKey, TValue>> newItems,
            IEnumerable<KeyValuePair<TKey, TValue>> updItems, IEnumerable<KeyValuePair<TKey, TValue>> delItems);
    }
}
