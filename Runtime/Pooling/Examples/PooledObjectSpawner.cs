using System.Collections;
using UnityEngine;
using Wolfey.Extensions;

namespace Wolfey.Pooling
{
    public class PooledObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObjectPool prefabPool;
        [SerializeField] float spawnDelay;
        [SerializeField] bool spawnOnEnable;

        Coroutine _spawnLoop;
        Transform _transform;

        void Awake()
        {
            _transform = transform;
        }

        void OnEnable()
        {
            
            if(!spawnOnEnable) return;
            _spawnLoop = StartCoroutine(SpawnLoop());
            
        }

        void OnDisable()
        {
            if(_spawnLoop == null) return;
            StopCoroutine(_spawnLoop);
            _spawnLoop = null;
        }

        IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnDelay);
                
                Spawn();
            }    
        }

        public void Spawn()
        {
            if(_transform.childCount == 0) return;

            Transform spawnTransform = _transform.RandomChild();

            prefabPool.Get().Transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
        }
    }
}
