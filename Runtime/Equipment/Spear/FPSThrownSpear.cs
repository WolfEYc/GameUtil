using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using Wolfey.Audio;
using Wolfey.Events;


namespace WolfeyFPS
{
    [RequireComponent(typeof(Rigidbody))]
    public class FPSThrownSpear : NetworkBehaviour
    {
        Rigidbody _rb;
        Vector3 _prevPos;
        Vector3 _thrownPos;
        float _currenttl;
        bool _embbedded;
        bool _thrown;
        Vector3 _embbedOffset;
        Quaternion _rotOffset;
        Transform _embeddedTransform;
        Transform _transform;
        
        [Header("Basic Refs")]
        [SerializeField] LayerMask damageMask;
        [SerializeField] float vel2Dmg;
        [SerializeField] float ttl;
        [SerializeField] EventObject preRaycast;
        [SerializeField] EventObject ownerHitResponse;
        
        [Header("Sounds")]
        [SerializeField] PlayRandomClip hitPlayer;
        [SerializeField] PlayRandomClip hitWorld;
        
        //[Header("VFX")]
        NetworkObject _thrower;
        Sprite _killIcon;

        public struct PreRaycastData
        {
             public NetworkConnection Owner;
             public Vector3 Pos;

             public PreRaycastData(NetworkConnection owner, Vector3 pos)
             {
                 Owner = owner;
                 Pos = pos;
             }
        }

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _transform = transform;
        }

        void OnEnable()
        {
            _currenttl = ttl;
        }

        void FixedUpdate()
        {
            if (!IsServer) return;
            
            _currenttl -= Time.fixedDeltaTime;

            if (_currenttl < 0f)
            {
                Despawn();
            }
            
            if(_embbedded) return;
            
            var currentPos = _rb.position;
            var direction = (currentPos - _prevPos).normalized;
            var distance = Vector3.Distance(_prevPos, currentPos);
            
            preRaycast.Invoke(new PreRaycastData(Owner, _rb.position));
            if (!Physics.Raycast(
                    _prevPos,
                    direction,
                    out RaycastHit hit,
                    distance,
                    damageMask,
                    QueryTriggerInteraction.Collide
                ))
            {
                _prevPos = currentPos;
                return;
            }
            
            _prevPos = currentPos;
            
            if (!ReferenceEquals(hit.rigidbody, null))
            {
                NetworkObject nob = hit.rigidbody.GetComponentInParent<NetworkObject>();
                
                if(nob.Owner == Owner) return;
                
                ImpactRb(
                    nob,
                    _rb.velocity.magnitude,
                    hit.rigidbody.position,
                    hit.point,
                    _thrower
                );
                return;
            }
            
            ImpactWorld(hit.point);
        }

        void LateUpdate()
        {
            if(!_embbedded || _embeddedTransform == null) return;
            _transform.SetPositionAndRotation(
                _embeddedTransform.position + _embbedOffset,
                _embeddedTransform.rotation * _rotOffset
            );
        }
        
        [ObserversRpc(BufferLast = true, RunLocally = true)]
        void ImpactRb(NetworkObject nob, float vel, Vector3 sourcePos, Vector3 hitPos, NetworkObject thrower)
        {
            DamageParent damageParent = nob.GetComponent<DamageParent>();
            damageParent.DamageAll(vel * vel2Dmg, _thrownPos, sourcePos, thrower, _killIcon);
            
            _embbedded = true;
            _rb.isKinematic = true;

            _embeddedTransform = damageParent.damageTransform;
            
            _embbedOffset = hitPos - _embeddedTransform.position;
            _rotOffset = Quaternion.Inverse(_embeddedTransform.rotation) * transform.rotation;
            hitPlayer.Play();

            if (IsOwner)
            {
                ownerHitResponse.Invoke();
            }
        }

        [ObserversRpc(BufferLast = true, RunLocally = true)]
        void ImpactWorld(Vector3 pos)
        {
            _embbedded = true;
            _rb.isKinematic = true;
            hitWorld.Play();
            transform.position = pos;
        }

        [ObserversRpc(BufferLast = true, ExcludeServer = true)]
        void ThrowObserver(float vel, FPSEquip equip)
        {
            PerformThrow(transform.position, vel, equip);
        }

        [Server]
        public void Throw(Vector3 pos, float vel, NetworkObject thrower, FPSEquip equip)
        {
            _thrower = thrower;
            ThrowObserver(vel, equip);
            PerformThrow(pos, vel, equip);
        }

        void PerformThrow(Vector3 pos, float vel, FPSEquip equip)
        {
            if(_embbedded || _thrown) return;
            _thrown = true;
            _killIcon = equip.Icon;
            var transform1 = transform;
            _currenttl = ttl;
            _embbedded = false;
            _prevPos = pos;
            _thrownPos = pos;
            _rb.velocity = transform1.forward * vel;
        }
    }
}
