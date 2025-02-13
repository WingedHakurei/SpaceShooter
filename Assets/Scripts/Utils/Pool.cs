using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils
{
    public class Pool<T> : IDisposable where T : Component
    {
        public static Pool<T> Instance;
        private readonly Dictionary<string, SinglePool> _pools = new();

        public Pool(string[] names, T[] prefabs)
        {
            for (var i = 0; i < prefabs.Length; i++)
            {
                var key = i < names.Length ? names[i] : prefabs[i].name;
                _pools[key] = new SinglePool(prefabs[i]);
            }
        }

        public static T Get(string key)
        {
            if (Instance == null)
            {
                throw new NullReferenceException("The pool object is null.");
            }

            return Instance.InstanceGet(key);
        }

        private T InstanceGet(string key)
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                return pool.Get();
            }
            else
            {
                throw new KeyNotFoundException("The key " + key + " was not found in the pool.");
            }
        }

        public static void Collect(string key, T obj)
        {
            if (Instance == null)
            {
                throw new NullReferenceException("The pool object is null.");
            }

            Instance.InstanceCollect(key, obj);
        }
        
        private void InstanceCollect(string key, T obj)
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                pool.Collect(obj);
            }
            else
            {
                throw new KeyNotFoundException("The key " + key + " was not found in the pool.");
            }
        }

        public void Dispose()
        {
            foreach (var pool in _pools.Values)
            {
                pool.Dispose();
            }
            
            Instance = null;
        }

        private class SinglePool : IDisposable
        {
            private T _prefab;
            private readonly Stack<T> _container = new();

            public SinglePool(T prefab)
            {
                _prefab = prefab;
            }

            public void Dispose()
            {
                while (_container.Count > 0)
                {
                    var gameObject = _container.Pop();
                    if (gameObject)
                    {
                        Object.DestroyImmediate(gameObject);
                    }
                }
                _prefab = null;
            }

            public T Get()
            {
                T obj;
                if (_container.Count > 0)
                {
                    obj = _container.Pop();
                }
                else
                {
                    obj = Object.Instantiate(_prefab);
                    obj.gameObject.SetActive(false);
                }
                return obj;
            }

            public void Collect(T obj)
            {
                if (!obj)
                {
                    Debug.Log(1);
                }
                obj.gameObject.SetActive(false);
                _container.Push(obj);
            }
        }
    }
}