using FishNet.Object;
using UnityEngine;
using Wolfey.Audio;

namespace WolfeyFPS
{
    [RequireComponent(typeof(FPSEquip))]
    public class FPSSpear : NetworkBehaviour
    {
        [SerializeField] NetworkObject spear;
        [SerializeField] float spearVel;
        [SerializeField] bool canThrow;
        [SerializeField] float speedMult;
        [SerializeField] PlayRandomClip throwSound;
        
        FPSEquip _equip;
        Animator Hands => _equip.WeaponHandler.Hands;
        Transform Cam => _equip.WeaponHandler.camTransform;

        static readonly int AttackState = Animator.StringToHash("HandsLayer.Attack");
        static readonly int IdleState = Animator.StringToHash("HandsLayer.IdleEquip");
        static readonly int MinusState = Animator.StringToHash("HandsLayer.MinusAttack");
        
        
        void Awake()
        {
            _equip = GetComponent<FPSEquip>();
            _equip.OnPlusAttack += PrepThrow;
            _equip.OnMinusAttack += Throw;
        }
        
        [ObserversRpc(ExcludeOwner = true)]
        void IdleObserver()
        {
            Hands.Play(IdleState);
        }
        
        [ObserversRpc(ExcludeOwner = true)]
        void ThrowObserver()
        {
            Hands.Play(MinusState);
            throwSound.Play();
            _equip.Visuals.SetActive(false);
        }  
        
        [ServerRpc]
        void ThrowServer(Vector3 pos, Vector2 rot, float vel)
        {
            ThrowObserver();
            NetworkObject thrownSpear = Instantiate(spear, pos, Quaternion.Euler(rot.x, rot.y, 0f));
            Spawn(thrownSpear, Owner);
            FPSThrownSpear thrownSpearComp = thrownSpear.GetComponent<FPSThrownSpear>();
            thrownSpearComp.Throw(pos, vel, _equip.WeaponHandler.NetworkObject, _equip);
        }

        [ServerRpc]
        void IdleServer()
        {
            IdleObserver();
        }
        
        void Throw()
        {
            _equip.WeaponHandler.UpdateMouseHeld();

            if (Hands.GetCurrentAnimatorStateInfo(1).fullPathHash != AttackState)
            {
                IdleServer();
                return;
            }
            
            if (!canThrow)
            {
                Hands.ResetTrigger(FPSWeaponHandler.SwitchReady);
                Hands.SetTrigger(FPSWeaponHandler.MinusAttack);
                IdleServer();
                return;
            }
            
            Hands.SetTrigger(FPSWeaponHandler.MinusAttack);
            
            ThrowServer(Cam.position, Cam.rotation.eulerAngles, spearVel);
            
            throwSound.Play();
            _equip.Visuals.SetActive(false);
        }

        [ObserversRpc(ExcludeOwner = true)]
        void PrepThrowObserver()
        {
            Hands.Play(AttackState);
        }
        
        [ServerRpc]
        void PrepThrowServer()
        {
            PrepThrowObserver();
        }
        
        [Client(RequireOwnership = true)]
        void PrepThrow()
        {
            _equip.WeaponHandler.UpdateMouseHeld();
            if(Hands.GetCurrentAnimatorStateInfo(1).fullPathHash == AttackState) return;
            
            PrepThrowServer();
            Hands.SetTrigger(FPSWeaponHandler.Attack);
        }

        void FixedUpdate()
        {
            if (!IsOwner || ReferenceEquals(_equip.WeaponHandler, null))
            {
                return;
            }

            if (!_equip.Equipped || !canThrow)
            {
                _equip.WeaponHandler.controller.maxSpeedMult = 1f;
                return;
            }
            
            _equip.WeaponHandler.controller.maxSpeedMult = speedMult;
        }
    }
}
