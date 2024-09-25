using System;
using UnityEngine;

namespace ObjectPoolingSystem
{
    public interface IPoolable<T>
    {
        void Initialize(Transform parent, Action<PoolObject> resetAction);
        void Reset(Transform parent);
    }
}