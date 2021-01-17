using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
    public class TypeCache<TSearch, TStorage> where TStorage : Attribute
    {
        public delegate bool TypeCachePredicate(Type type, ref TStorage storage, out Type key);
        
        private bool _initialized;
        private readonly Dictionary<Type, TStorage> _cache;
        private readonly TypeCachePredicate _predicate;
        
        public IEnumerable<string> Names
        {
            get
            {
                ShouldBuildCache();
                foreach (var type in _cache.Keys)
                {
                    yield return type.FullName;
                }
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

        public TStorage this[Type key]
        {
            get
            {
                ShouldBuildCache();
                return _cache[key];
            }
        }

        public TypeCache(TypeCachePredicate predicate)
        {
            _initialized = false;
            _cache = new Dictionary<Type, TStorage>();
            _predicate = predicate;
        }

        public TStorage Get<T>() => this[typeof(T)];
        
        public bool TryGet(Type type, out TStorage output)
        {
            ShouldBuildCache();
            return _cache.TryGetValue(type, out output);
        }
        
        private void ShouldBuildCache(bool force = false)
        {
            if (!_initialized || force) BuildCache();
        }

        private void BuildCache()
        {
            foreach (var type in TypeExtensions.GetAllTypes(typeof(TSearch)))
            {
                foreach (var attribute in type.GetCustomAttributes<TStorage>())
                {
                    var storage = attribute;
                    if (_predicate(type, ref storage, out var key))
                    {
                        _cache.Add(key, storage);
                    }
                }
            }

            _initialized = true;
        }
    }
}