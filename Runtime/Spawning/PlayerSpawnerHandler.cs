using System.Collections;
using FishNet.Object;
using UnityEngine;
using Wolfey.Events;
using WolfeyFPS;

namespace SpearMonkey
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerSpawnerHandler : NetworkBehaviour
    {
        [SerializeField] EventObject selfSpawnEvent, globalSpawnEvent;
        [SerializeField] FPSController controller;
        public float addedSpawnDelay;

        Transform _transform;
        Rigidbody _rb;

        NetworkEventData _spawnData;
        SpawnPoint _latestSpawnPoint;

        public bool Dead { get; set; }
        bool _spawnPause;

        public void SetSpawnPause(bool spawnPause)
        {
            _spawnPause = spawnPause;
            StopAllCoroutines();
        }

        void Awake()
        {
            _transform = transform;
            _rb = GetComponent<Rigidbody>();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            GamemodeSwitcher.protectedNobs.Add(NetworkObject);
            ServerPerformSpawn();
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            GamemodeSwitcher.protectedNobs.Remove(NetworkObject);
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            _spawnData = new NetworkEventData(Owner);
        }

        void PerformSpawn(int idx)
        {
            Debug.Log("SPAWNING!!!!!!", gameObject);
            _latestSpawnPoint = SpawnsManager.Instance.SpawnPoints[idx];
            var position = _latestSpawnPoint.Pos;
            
            if (Owner.IsLocalClient)
            {
                _rb.velocity = Vector3.zero;
                controller.SetRot(_latestSpawnPoint.Rot);
                controller.hitFricMult = 0f;
                selfSpawnEvent.Invoke(_spawnData);
            }
            
            _rb.position = position;
            _transform.position = position; 
            globalSpawnEvent.Invoke(_spawnData);
            Dead = false;
        }
        
        void SwitchSpawn(int idx)
        {
            Debug.Log("SWITCH SPAWNING!!!!!!", gameObject);
            _latestSpawnPoint = SpawnsManager.Instance.SpawnPoints[idx];
            var position = _latestSpawnPoint.Pos;
            
            if (IsOwner)
            {
                _rb.velocity = Vector3.zero;
                controller.SetRot(_latestSpawnPoint.Rot);
                controller.hitFricMult = 0f;
            }
            
            _rb.position = position;
            _transform.position = position;
            Dead = false;
            _spawnPause = false;
        }
        
        [ObserversRpc(BufferLast = true)]
        void ObserversPerformSpawn(int idx)
        {
            PerformSpawn(idx);
        }
        
        [ObserversRpc(BufferLast = true)]
        void ObserversSwitchSpawn(int idx)
        {
            SwitchSpawn(idx);
        }

        public void ServerPerformSpawn()
        {
            if(!IsServer) return;
            int spawnIdx = SpawnsManager.Instance.GenerateSpawnPoint();
            ObserversPerformSpawn(spawnIdx);
        }

        public void ServerSwitchPos()
        {
            if(!IsServer) return;
            int spawnIdx = SpawnsManager.Instance.GenerateSpawnPoint();
            ObserversSwitchSpawn(spawnIdx);
        }

        [Server]
        IEnumerator SpawnDelayRoutine()
        {
            yield return new WaitForSeconds(addedSpawnDelay + SpawnDelay.Instance.spawnDelay);
            if(!Dead || _spawnPause) yield break;
            ServerPerformSpawn();
        }

        public void OnDeath()
        {
            Dead = true;
            if(!IsServer) return;
            StartCoroutine(SpawnDelayRoutine());
        }
    }
}
