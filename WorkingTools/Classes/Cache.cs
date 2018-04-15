using System;
using System.Collections.Generic;
using WorkingTools.Extensions;

namespace WorkingTools.Classes
{
    public class Cache<TKey, TValue> : CacheLite<TKey, TValue>
    {
        public Cache(Func<TKey, TValue> get) : base(get) { }

        public virtual bool Contains(TKey key)
        { lock (Lock) return DictionaryKesh.ContainsKey(key); }

        public virtual void Set(TKey key, TValue value)
        {
            lock (Lock)
                if (DictionaryKesh.ContainsKey(key))
                    DictionaryKesh[key] = value;
                else
                    DictionaryKesh.Add(key, value);
        }

        public virtual void Clear()
        { lock (Lock) DictionaryKesh.Clear(); }

        public virtual ICollection<KeyValuePair<TKey, TValue>> Items { get { return DictionaryKesh; } }
    }


    public class CacheLite<TKey, TValue>
    {
        protected readonly object Lock = new object();

        protected readonly Dictionary<TKey, TValue> DictionaryKesh = new Dictionary<TKey, TValue>();
        protected readonly Func<TKey, TValue> GetValueByKey;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="get">функция получения значения по ключу</param>
        public CacheLite(Func<TKey, TValue> get)
        {
            if (get == null) throw new ArgumentOutOfRangeException("get", "функция получения значения по ключу не может быть null");

            GetValueByKey = get;
        }

        public virtual TValue Get(TKey key)
        {
            lock (Lock)
                if (!DictionaryKesh.ContainsKey(key))
                    return GetValueByKey(key).Do(value => DictionaryKesh.Add(key, value));
                else
                    return DictionaryKesh[key];
        }
    }
}
