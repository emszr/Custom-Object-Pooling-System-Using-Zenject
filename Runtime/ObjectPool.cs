using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace ObjectPoolingSystem
{
    public class ObjectPool
    {
        private readonly Stack<PoolObject> _pooledObjects = new();
        private readonly HashSet<PoolObject> _activeObjects = new();
        private PoolObject _prefab;

        [Inject] private DiContainer _diContainer;
        [Inject] private ObjectPoolContainer _objectPoolContainer;

        public int PooledObjectCount => _pooledObjects.Count;
        public int ActiveObjectCount => _activeObjects.Count;

        public void Initialize(Transform parent, PoolObject prefab, int initialObjectCount)
        {
            _prefab = prefab;
            
            RestoreObjects();

            for (var i = 0; i < Math.Max(0, initialObjectCount - _pooledObjects.Count); i++)
            {
                var poolObject = Object.Instantiate(_prefab);
                ResetPoolObject(parent, poolObject);
            }
        }

        public PoolObject GetPoolObject(Transform parent, Action<PoolObject> getPoolObjectAction = null, Action<PoolObject> resetPoolObjectAction = null)
        {
            var poolObject = PooledObjectCount > 0 ? _pooledObjects.Pop() : Object.Instantiate(_prefab);
            _activeObjects.Add(poolObject);
            _diContainer.InjectGameObject(poolObject.gameObject);
            poolObject.Initialize(parent, resetPoolObjectAction);
            getPoolObjectAction?.Invoke(poolObject);
            return poolObject;
        }

        public void ResetPoolObject(Transform parent, PoolObject poolObject)
        {
            _pooledObjects.Push(poolObject);
            _activeObjects.Remove(poolObject);
            poolObject.Reset(parent);
        }

        public void Reset(Transform parent)
        {
            foreach (var activeObject in _activeObjects)
            {
                _pooledObjects.Push(activeObject);
                activeObject.Reset(parent);
            }

            _activeObjects.Clear();
        }

        public void StoreObjects()
        {
            _objectPoolContainer.PoolObjects.AddRange(_activeObjects);
            _objectPoolContainer.PoolObjects.AddRange(_pooledObjects);
        }

        public void RestoreObjects()
        {
            _pooledObjects.Clear();
            
            foreach (var poolObject in _objectPoolContainer.PoolObjects.FindAll(x => x.poolObjectType == _prefab.poolObjectType))
            {
                _pooledObjects.Push(poolObject);
            }
        }

        public class Factory : PlaceholderFactory<ObjectPool>
        {
        }
    }
}