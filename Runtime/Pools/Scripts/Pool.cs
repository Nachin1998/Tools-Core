using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Pool;

namespace Nach.Tools.Pools
{
    [Serializable]
    public class Pool<T> where T : Component
    {
        [SerializeField] private T prefabToPool = null;
        [SerializeField] private Transform holder = null;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxSize = 1000;
        [SerializeField] private bool checkForInternalPoolErrors = true;

        private ObjectPool<T> pool = null;
        private List<T> items = null;

        public int ActiveItemsCount => items != null ? items.Count : 0;

        public Pool(T prefabToPool, Transform holder = null, int defaultCapacity = 10, int maxSize = 1000, bool autoInit = true)
        {
            SetPrefabToPool(prefabToPool);
            SetItemsHolder(holder);
            this.defaultCapacity = defaultCapacity;
            this.maxSize = maxSize;

            if (autoInit)
            {
                Init();
            }
        }

        public void Init()
        {
            items = new List<T>();
            pool = new ObjectPool<T>(CreateItem, GetItem, ReleaseItem, OnDestroyItem, checkForInternalPoolErrors, defaultCapacity, maxSize);
        }

        public T Get()
        {
            return pool.Get();
        }

        public void Release(T item)
        {
            pool.Release(item);
        }

        public void Clear()
        {
            List<T> copy = new List<T>(items);

            foreach (T poolable in copy)
            {
                pool.Release(poolable);
            }

            items.Clear();
        }

        public void SetItemsHolder(Transform holder)
        {
            this.holder = holder;
        }

        public void SetPrefabToPool(T prefabToPool)
        {
            this.prefabToPool = prefabToPool;
        }

        public T GetActiveItem(int index)
        {
            return items[index];
        }

        private T CreateItem()
        {
            T item = GameObject.Instantiate(prefabToPool, holder);
            if (item is IPoolable poolable)
            {
                poolable.OnCreated();
            }
            else
            {
                throw new InvalidOperationException("You are trying to pool a non-IPoolable item. " +
                                                    "Make sure the object you are trying to pool implements the IPoolable interface.");
            }
            return item;
        }

        private void GetItem(T item)
        {
            if (item is IPoolable poolable)
            {
                poolable.OnGet();
                items.Add(item);
            }
            else
            {
                throw new InvalidOperationException("You are trying to pool a non-IPoolable item. " +
                                                    "Make sure the object you are trying to pool implements the IPoolable interface.");
            }
        }

        private void ReleaseItem(T item)
        {
            if (item is IPoolable poolable)
            {
                poolable.OnRelease();
                items.Remove(item);
            }
            else
            {
                throw new InvalidOperationException("You are trying to pool a non-IPoolable item. " +
                                                    "Make sure the object you are trying to pool implements the IPoolable interface.");
            }
        }

        private void OnDestroyItem(T t)
        {
            GameObject.Destroy(t.gameObject);
        }
    }
}
