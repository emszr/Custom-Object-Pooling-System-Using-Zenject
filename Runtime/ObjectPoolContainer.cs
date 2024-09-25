using System.Collections.Generic;
using UnityEngine;

namespace ObjectPoolingSystem
{
    public class ObjectPoolContainer : MonoBehaviour
    {
        public readonly List<PoolObject> PoolObjects = new();
    }
}