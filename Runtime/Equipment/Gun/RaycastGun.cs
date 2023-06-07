using FishNet.Object;
using UnityEngine;
using Wolfey;
using Wolfey.Pooling;

namespace WolfeyFPS
{
    [RequireComponent(typeof(FPSGun))]
    public class RaycastGun : NetworkBehaviour, IGun
    {
        [SerializeField] Transform muzzle;
        [SerializeField] Transform casingEjectPort;
        [SerializeField] float casingEjectSpeed;
        [SerializeField] GameObjectPool shotHolePool;
        [SerializeField] GameObjectPool tracerPool;
        [SerializeField] GameObjectPool casingPool;

        FPSGun _fpsGun;
        int _shotCount;
        Camera _fpsCam;

        Transform CamTransform => _fpsGun.Equip.WeaponHandler.camTransform;
        LayerMask DamageableLayers => _fpsGun.Equip.WeaponHandler.damageInflictLayer;
        int TracerEvery => _fpsGun.gunInfo.tracerEvery;
        bool ShouldFireTracer => TracerEvery > 0 && _shotCount % TracerEvery == 0;
        float MaxRange => _fpsGun.gunInfo.maxRange;

        Rigidbody InheritedRb => _fpsGun.Equip.WeaponHandler.rb;

        void Awake()
        {
            _fpsGun = GetComponent<FPSGun>();
            _fpsGun.Gun = this;
            _fpsCam = Camera.main;
        }

        void PlaceShotHole(Vector3 hitPoint, Vector3 normal)
        {
            //finish this stub
            Transform shotHole = shotHolePool.Get().Transform;
            
            shotHole.position = hitPoint + normal * 0.01f;
            shotHole.forward = normal;
        }

        [ObserversRpc(ExcludeOwner = true)]
        void PlaceShotHoleObserver(Vector3 hitPoint, Vector3 normal)
        {
            PlaceShotHole(hitPoint, normal);
        }

        [ServerRpc]
        void PlaceShotHoleServer(Vector3 hitPoint, Vector3 normal)
        {
            PlaceShotHoleObserver(hitPoint, normal);
        }
        
        void TryPlaceShotHole(Vector3 hitpoint, Vector3 normal)
        {
            PlaceShotHole(hitpoint, normal);
            PlaceShotHoleServer(hitpoint, normal);
        }

        void FireTracer(Vector3 hitPoint)
        {
            PooledObject pooledObject = tracerPool.Get();
            TracerRound tracerRound = pooledObject.GetComponent<TracerRound>();
            tracerRound.Init(muzzle.position, hitPoint);
        }

        [ObserversRpc(ExcludeOwner = true)]
        void FireTracerObserver(Vector3 hitPoint)
        {
            FireTracer(hitPoint);
        }
        
        [ServerRpc]
        void FireTracerServer(Vector3 hitPoint)
        {
            FireTracerObserver(hitPoint);
        }
        
        void TryFireTracer(Vector3 hitPoint)
        {
            if(!ShouldFireTracer) return;
            
            FireTracer(hitPoint);
            FireTracerServer(hitPoint);
        }

        void EjectCasing()
        {
            Transform casing = casingPool.Get().Transform;
            Rigidbody rb = casing.GetComponent<Rigidbody>();
            
            casing.SetPositionAndRotation(_fpsCam.PosAtFOV(casingEjectPort.position, 60f), casingEjectPort.rotation);
            rb.velocity = InheritedRb.velocity + casingEjectPort.right * casingEjectSpeed;
        }

        [ObserversRpc(ExcludeOwner = true)]
        void EjectCasingObserver()
        {
            EjectCasing();
        }

        [ServerRpc]
        void EjectCasingServer()
        {
            EjectCasingObserver();
        }
        
        void TryEjectCasing()
        {
            EjectCasing();
            EjectCasingServer();
        }

        public void Shoot(Vector3 dir)
        {
            _shotCount++;
            TryEjectCasing();

            if (!Physics.Raycast(
                    CamTransform.position,
                    dir,
                    out RaycastHit hit,
                    MaxRange,
                    DamageableLayers
                ))
            {
                TryFireTracer(CamTransform.position + CamTransform.forward * MaxRange);
                return;
            }
            
            TryFireTracer(hit.point);

            if (ReferenceEquals(hit.rigidbody, null))
            {
                TryPlaceShotHole(hit.point, hit.normal);
                return;
            }

            float dmg = _fpsGun.gunInfo.dmgPerShot;

            if (hit.distance > _fpsGun.gunInfo.dmgFalloffRange)
            {
                float distanceMult = 1f - hit.distance.Remap(0f, MaxRange, 0f, 1f);
                
                dmg *= distanceMult * _fpsGun.gunInfo.dmgFalloffMult;
            }

            foreach (var damageable in hit.rigidbody.GetComponentsInChildren<IDamageable>())
            {
                //damageable.Damage(dmg, hit.point, CamTransform.position, NetworkObject);
            }
        }
    }
}
