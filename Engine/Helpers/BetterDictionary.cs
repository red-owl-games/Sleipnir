using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    [Serializable]
    public class BetterDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] 
        private List<TKey> _keys;
        [SerializeField] 
        private List<TValue> _values;

        public BetterDictionary() : base() { }
        public BetterDictionary(int capacity) : base(capacity) { }
        public BetterDictionary(IEqualityComparer<TKey> comparer) : base(0, comparer) { }
        public BetterDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
        public BetterDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary, null) { }
        public BetterDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary?.Count ?? 0, comparer) { }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            foreach (var value in Values)
            {
                if (value is ISerializationCallbackReceiver callback) callback.OnBeforeSerialize();
            }
            _keys = new List<TKey>(Keys);
            _values = new List<TValue>(Values);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            int count = _keys.Count;
            Clear();
            for (int i = 0; i < count; i++) {
                this[_keys[i]] = _values[i];
            }
        }
    }
}