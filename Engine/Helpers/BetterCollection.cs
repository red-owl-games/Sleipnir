using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedOwl.Sleipnir.Engine
{
     [Serializable]
     public abstract class BetterCollection<T> : ICollection<T>
     {
         [SerializeField]
         private List<T> _collection;

         protected BetterCollection()
         {
             _collection = new List<T>();
         }

         protected BetterCollection(int capacity)
         {
             _collection = new List<T>(capacity);
         }

         public int Count => _collection.Count;

         public bool IsReadOnly => false;

         public void Clear() => _collection.Clear();

         public bool Contains(T item) => _collection.Contains(item);

         public void CopyTo(T[] array, int index) => _collection.CopyTo(array, index);

         public void Add(T item) => _collection.Add(item);

         public void Remove(T value)
         {
             for (int i = _collection.Count - 1; i >= 0; i--)
             {
                 if (_collection[i].Equals(value))
                 {
                     Debug.Log($"Removing Index {i}");
                     _collection.RemoveAt(i);
                 }
             }
         }

         public void Remove(int index) => _collection.RemoveAt(index);
         
         bool ICollection<T>.Remove(T item) => _collection.Remove(item);
         
         public IEnumerator<T> GetEnumerator() => _collection.GetEnumerator();

         IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
     }
}