using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public interface ITypeStorage
    {
        bool ShouldCache(Type type);
    }
    
    public class TypeCache<T, TStorage> where TStorage : ITypeStorage, new()
    {
        private Dictionary<string, TStorage> _cache;
        
        public IEnumerable<string> Names
        {
            get
            {
                ShouldBuildCache();
                return _cache.Keys;
            }
        }
        
        public IEnumerable<TStorage> All
        {
            get
            {
                ShouldBuildCache();
                return _cache.Values;
            }
        }

        public bool Get(Type type, out TStorage output)
        {
            ShouldBuildCache();
            return _cache.TryGetValue(type.SafeGetName(), out output);
        }
        
        public void ShouldBuildCache(bool force = false)
        {
            if (_cache == null || force) BuildCache();
        }

        private void BuildCache()
        {
            _cache = new Dictionary<string, TStorage>();
            foreach (var type in TypeExtensions.GetAllTypes<T>())
            {
                var storage = new TStorage();
                if (storage.ShouldCache(type))
                {
                    //Debug.Log($"Caching '{type.SafeGetName()}'");
                    _cache.Add(type.SafeGetName(), storage);
                }
            }
        }
    }
}