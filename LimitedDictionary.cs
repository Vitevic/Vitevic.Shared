using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vitevic.Shared
{
    public class LimitedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dictionary;
        private Queue<TKey> _keys;

        public LimitedDictionary(uint maxCapacity)
        {
            _dictionary = new Dictionary<TKey, TValue>();
            _keys = new Queue<TKey>();
            MaxCapacity = maxCapacity;
        }

        public uint MaxCapacity { get; set; }

        #region IDictionary<TKey, TValue>

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public ICollection<TKey> Keys => _dictionary.Keys;
        public ICollection<TValue> Values => _dictionary.Values;
        public int Count => _dictionary.Count;
        public bool IsReadOnly => ((IDictionary<TKey, TValue>)_dictionary).IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            if( _dictionary.Count >= MaxCapacity )
            {
                var oldestKey = _keys.Dequeue();
                _dictionary.Remove(oldestKey);
            }

            _dictionary.Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
            _keys.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>)_dictionary).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            var res = _dictionary.Remove(key);
            if( res )
            {
                _keys = new Queue<TKey>( _keys.Where(x => !object.Equals(x,key)) );
            }
            return res;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion IDictionary<TKey, TValue>
    }
}
