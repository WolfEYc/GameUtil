using Cinemachine;
using FishNet.Object;
using UnityEngine;
using Wolfey.Audio;
using Wolfey.Pooling;

namespace WolfeyFPS
{
    [RequireComponent(typeof(NetworkTimer))]
    public class HotPotato : NetworkBehaviour
    {
        [SerializeField] float slapDist;
        [SerializeField] PlayRandomClip slapSound;
        [SerializeField] CinemachineImpulseSource impulseSource;
        [SerializeField] GameObjectPool explosionSystem;
        [SerializeField] LayerMask slapMask;
        [SerializeField] GameObject sparklers;

        FPSEquip _equip;
        Animator Hands => _equip.WeaponHandler.Hands;
        Transform Cam => _equip.WeaponHandler.camTransform;

        static readonly int AttackState = Animator.StringToHash("HandsLayer.Attack");
        
        void Awake()
        {
            _equip = GetComponent<FPSEquip>();
            
            _equip.OnPickup += EquipOnOnPickup;
            _equip.OnEquip += OnEquip;
            _equip.OnUnEquip += EquipOnOnUnEquip;
            _equip.OnPlusAttack += Throw;
            _equip.OnPerformAction += Shoot;
        }
        

        void EquipOnOnPickup()
        {
            slapSound.Play();
            impulseSource.GenerateImpulse();
        }

        void EquipOnOnUnEquip()
        {
            _equip.WeaponHandler.UpdateMouseHeld();
        }

        void OnEquip()
        {
            Hands.ResetTrigger(FPSWeaponHandler.Attack);
            Hands.SetBool(FPSWeaponHandler.Held, true);
        }

        bool TryGetThrowTarget(out RaycastHit hit)
        {
            return Physics.Raycast(Cam.position, Cam.forward, out hit, slapDist, slapMask, QueryTriggerInteraction.Collide);
        }
        
        void ThrowObserver()
        {
            Hands.Play(AttackState);
        }  
        
        [ServerRpc]
        void ThrowServer(NetworkObject slappedBoi)
        {
            ThrowObserver();

            _equip.Pickup(slappedBoi);
        }

        void Throw()
        {
            if(!Hands.GetBool(FPSWeaponHandler.SwitchReady)) return;
            Hands.SetTrigger(FPSWeaponHandler.Attack);
        }

        [Client(RequireOwnership = true)]
        void Shoot()
        {
            if(!TryGetThrowTarget(out RaycastHit hitPlayer)) return;
            
            ThrowServer(hitPlayer.rigidbody.GetComponentInParent<NetworkObject>());
        }
        
        public void Explode()
        {
            explosionSystem.Get().Transform.position = _equip.WeaponHandler.camTransform.position;
            
            if (Owner.IsLocalClient)
            {
                _equip.WeaponHandler.killDist.NotifyServerOfDeath();
            }
            
            _equip.Visuals.SetActive(false);
            sparklers.SetActive(false);
            
            
            
            _equip.PerformDitch();
        }
    }
}
