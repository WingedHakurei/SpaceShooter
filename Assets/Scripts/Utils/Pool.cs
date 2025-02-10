using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils
{
    public class Pool : IDisposable
    {
        public static Pool Instance;
        private readonly Dictionary<string, SinglePool> _pools = new();
        private readonly Stack<GameObject> _fallback = new();

        public Pool(string[] names, GameObject[] prefabs)
        {
            for (var i = 0; i < prefabs.Length; i++)
            {
                var key = i < names.Length ? names[i] : prefabs[i].name;
                _pools[key] = new SinglePool(prefabs[i]);
            }
        }

        public static GameObject Get(string key)
        {
            if (Instance == null)
            {
                throw new NullReferenceException("The pool object is null.");
            }

            return Instance.InstanceGet(key);
        }

        private GameObject InstanceGet(string key)
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                return pool.Get();
            }

            GameObject obj;
            if (_fallback.Count > 0)
            {
                obj = _fallback.Pop();
            }
            else
            {
                obj = new GameObject(key);
                obj.SetActive(false);
            }
            return obj;
        }

        public static void Collect(string key, GameObject obj)
        {
            if (Instance == null)
            {
                throw new NullReferenceException("The pool object is null.");
            }

            Instance.InstanceCollect(key, obj);
        }
        
        private void InstanceCollect(string key, GameObject obj)
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                pool.Collect(obj);
            }
            else
            {
                _fallback.Push(obj);
            }
        }

        public void Dispose()
        {
            while (_fallback.Count > 0)
            {
                var obj = _fallback.Pop();
                if (obj)
                {
                    Object.Destroy(obj);
                }
            }

            foreach (var pool in _pools.Values)
            {
                pool.Dispose();
            }
            
            Instance = null;
        }

        private class SinglePool : IDisposable
        {
            private GameObject _prefab;
            private readonly Stack<GameObject> _container = new();

            public SinglePool(GameObject prefab)
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

            public GameObject Get()
            {
                GameObject obj;
                if (_container.Count > 0)
                {
                    obj = _container.Pop();
                }
                else
                {
                    obj = Object.Instantiate(_prefab);
                    obj.SetActive(false);
                }
                return obj;
            }

            public void Collect(GameObject obj)
            {
                obj.SetActive(false);
                _container.Push(obj);
            }
        }
    }
}