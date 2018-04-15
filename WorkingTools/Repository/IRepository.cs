using System;
using System.Collections.Generic;

namespace WorkingTools.Repository
{
    public interface IRepository<TKey, TValue> : IDisposable
    {
        bool Contains(TKey key);
        IEnumerable<KeyValuePair<TKey, TValue>> Get();
        bool Get(TKey key, out TValue value);
        TValue Get(TKey key);
        void Set(TKey key, TValue value);
        bool Remove(TKey key);
        void Clear();
        void Load();
        void Save();
    }
}
