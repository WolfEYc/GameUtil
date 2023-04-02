using UnityEngine;

namespace WolfeyGamedev.Pooling
{
    public class PooledObject : MonoBehaviour
    {
        public Transform Transform { get; private set; }
        public GameObject GameObject { get; private set; }
        public GameObjectPool GameObjectPool { get; set; }

        void Awake()
        {
            //Caching because pooled object will see frequent usage of these
            GameObject = gameObject;
            Transform = transform;
        }

        public void Release()
        {
            GameObjectPool.Release(this);
        }
    }
}
