using UnityEngine;
using UnityEngine.Pool;

namespace Wolfey.Pooling
{
    [CreateAssetMenu(order = 2, menuName = "WolfeyGamedev/GameObjectPool", fileName = "NewGameObjectPool")]
    public class GameObjectPool : ScriptableObject
    {
        [SerializeField] PooledObject prefab;

        Transform _poolParent;
        ObjectPool<PooledObject> _objectPool;
        
        public PooledObject Get()
        {
            _objectPool ??= new ObjectPool<PooledObject>(CreateClone, OnGet, OnRelease);
            return _objectPool.Get();
        }
        
        public void Release(PooledObject go)
        {
            _objectPool.Release(go);
        }

        public int CountInactive => _objectPool.CountInactive;
        public int CountActive => _objectPool.CountActive;
        public int CountAll => _objectPool.CountAll;
        
        void LazyInstantiateParentGameObject()
        {
            _poolParent = new GameObject($"Pool: {prefab.name}").transform;
            Application.quitting += _objectPool.Clear;
        }

        PooledObject CreateClone()
        {
            if (_poolParent == null)
            {
                LazyInstantiateParentGameObject();
            }

            PooledObject pooledObject = Instantiate(prefab, _poolParent);
            pooledObject.GameObjectPool = this;
            
            return pooledObject;
        }

        void OnRelease(PooledObject obj)
        {
            obj.GameObject.SetActive(false);
        }

        void OnGet(PooledObject obj)
        {
            obj.GameObject.SetActive(true);
        }

        void ApplicationOnQuitting()
        {
            Application.quitting -= ApplicationOnQuitting;
            _objectPool?.Clear();
        }
    }
}
