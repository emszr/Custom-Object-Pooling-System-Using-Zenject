using System;
using UnityEngine;

namespace ObjectPoolingSystem
{
    public abstract class PoolObject : MonoBehaviour, IPoolable<PoolObject>
    {
        public PoolObjectType poolObjectType;

        private Action<PoolObject> _resetAction;
        
        public virtual void Initialize(Transform parent, Action<PoolObject> resetAction)
        {
            gameObject.SetActive(true);
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;
            _resetAction = resetAction;
        }

        public virtual void Reset(Transform parent)
        {
            gameObject.SetActive(false);
            transform.SetParent(parent);
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.zero;
            transform.rotation = Quaternion.identity;
            _resetAction?.Invoke(this);
        }
    }
}