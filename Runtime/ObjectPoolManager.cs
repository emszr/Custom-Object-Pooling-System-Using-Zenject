using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ObjectPoolingSystem
{ 
    public class ObjectPoolManager : MonoBehaviour
    {
        public bool IsInitialized { get; private set; }
        
        [Inject] private ObjectPool.Factory _objectPoolFactory;
        [Inject] private ObjectPoolContainer _objectPoolContainer;
        
        [SerializeField] private int initialObjectCount;
        [SerializeField] private List<PoolObject> poolItemPrefabs;
        
        private readonly Dictionary<PoolObjectType, ObjectPool> _poolTypeTpPoolItemDictionary = new();
        
        private Transform PooledObjectsParent => _objectPoolContainer.transform;
        
        public void Initialize()
        {
            CreatePools(initialObjectCount);
            
            IsInitialized = true;
        }

        private void CreatePools(int initialCount)
        {
            foreach (var poolObject in poolItemPrefabs)
            {
                var objectPool = _objectPoolFactory.Create();
                objectPool.Initialize(PooledObjectsParent, poolObject, initialCount);
                _poolTypeTpPoolItemDictionary.Add(poolObject.poolObjectType, objectPool);
            }
            
            _objectPoolContainer.PoolObjects.Clear();
        }

        public PoolObject GetObject(PoolObjectType poolObjectType, Transform parent = null)
        {
            return _poolTypeTpPoolItemDictionary[poolObjectType].GetPoolObject(parent);
        }

        public int GetActiveObjectCountOfPool(PoolObjectType poolObjectType) => _poolTypeTpPoolItemDictionary[poolObjectType].ActiveObjectCount;
        
        public void ResetObject(PoolObject poolObject, Transform parent = null)
        {
            var pool = _poolTypeTpPoolItemDictionary[poolObject.poolObjectType];
            pool.ResetPoolObject(parent == null ? PooledObjectsParent : parent, poolObject);
        }

        public void ResetPools()
        {
            foreach (var poolTypeToPoolItem in _poolTypeTpPoolItemDictionary)
            {
                poolTypeToPoolItem.Value.StoreObjects();
                poolTypeToPoolItem.Value.Reset(_objectPoolContainer.transform);
            }

            foreach (var poolObject in _objectPoolContainer.PoolObjects)
            {
                poolObject.transform.SetParent(PooledObjectsParent);
            }
        }
    }
}