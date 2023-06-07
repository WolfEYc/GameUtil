using FishNet.Object;
using UnityEngine;
using Wolfey;
using Wolfey.Events;
using Wolfey.Pooling;


namespace WolfeyFPS
{
    [RequireComponent(typeof(Rigidbody))]
    public class Rocket : NetworkBehaviour
    {
        Rigidbody _rb;
        Vector3 _prevPos;
        float _currenttl;
        bool _thrown;
        Vector3 _embbedOffset;
        Quaternion _rotOffset;
        Transform _embeddedTransform;

        [SerializeField] LayerMask damageMask;
        [SerializeField] LayerMask explosionMask;
        [SerializeField] float dmg;
        [SerializeField] float ttl;
        [SerializeField] EventObject preRaycast;
        [SerializeField] EventObject ownerHitResponse;
        [SerializeField] float explosionRad;
        [SerializeField] GameObjectPool explosionPool;
        
        
        //[Header("VFX")]
        NetworkObject _thrower;
        Sprite _killIcon;
        
        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
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

            var currentPos = _rb.position;
            var direction = (currentPos - _prevPos).normalized;
            var distance = Vector3.Distance(_prevPos, currentPos);
            
            preRaycast.Invoke(new FPSThrownSpear.PreRaycastData(Owner, _rb.position));
            
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
            ImpactWorld(hit.point, _thrower);
            Despawn();
        }
        
        [ObserversRpc]
        void ImpactWorld(Vector3 pos, NetworkObject thrower)
        {
            explosionPool.Get().Transform.position = pos;

            Collider[] hits = Physics.OverlapSphere(pos, explosionRad, explosionMask, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < hits.Length; i++)
            {
                Rigidbody hitRb = hits[i].attachedRigidbody;
                
                DamageParent dmgParent = hitRb.GetComponentInParent<DamageParent>();
                Vector3 headPos = dmgParent.head.position;
                
                float dist = Vector3.Distance(headPos, pos);

                if(Physics.Raycast(
                       pos,
                       (headPos - pos).normalized,
                       out RaycastHit hit,
                    dist,
                    ~0,
                    QueryTriggerInteraction.Ignore) 
                   && hit.collider != hits[i]
                   ) continue;
                
                dmgParent.DamageAll(
                    Mathf.Lerp(1f, dmg, 1f - dist.Remap(0f, explosionRad, 0f, 1f)),
                    pos,
                    headPos,
                    thrower,
                    _killIcon);
            }

            if (hits.Length > 0)
            {
                ownerHitResponse.Invoke();
            }
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
            if(_thrown) return;
            _thrown = true;
            _killIcon = equip.Icon;
            var transform1 = transform;
            _currenttl = ttl;
            _prevPos = pos;
            _rb.velocity = transform1.forward * vel;
        }
    }
}
