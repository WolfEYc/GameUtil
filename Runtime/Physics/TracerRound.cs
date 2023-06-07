using System;
using UnityEngine;
using Wolfey.Pooling;

namespace WolfeyFPS
{
    [RequireComponent(typeof(PooledObject))]
    public class TracerRound : MonoBehaviour
    {
        PooledObject _pooledObject;
        const float TracerSpeed = 325f;

        Vector3 _target;

        void Awake()
        {
            _pooledObject = GetComponent<PooledObject>();
        }

        public void Init(Vector3 startPt, Vector3 hitPoint)
        {
            _target = hitPoint;
            _pooledObject.Transform.position = startPt;
            _pooledObject.Transform.LookAt(hitPoint, Vector3.up);
            ExecuteMove();
        }

        void ExecuteMove()
        {
            _pooledObject.Transform.position = Vector3.MoveTowards(
                _pooledObject.Transform.position,
                _target,
                Time.deltaTime * TracerSpeed
            );
            
            if(_pooledObject.Transform.position != _target) return;
            
            _pooledObject.Release();
        }

        void Update()
        {
            ExecuteMove();
        }
    }
}
