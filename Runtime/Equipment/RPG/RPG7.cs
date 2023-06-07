using FishNet.Object;
using UnityEngine;
using Wolfey.Audio;

namespace WolfeyFPS
{
    [RequireComponent(typeof(FPSEquip))]
    public class RPG7 : NetworkBehaviour
    {
        [SerializeField] NetworkObject rocket;
        [SerializeField] float spearVel;
        [SerializeField] PlayRandomClip shootSound;
        
        FPSEquip _equip;
        Animator Hands => _equip.WeaponHandler.Hands;
        Transform Cam => _equip.WeaponHandler.camTransform;

        static readonly int AttackState = Animator.StringToHash("HandsLayer.Attack");
        
        void Awake()
        {
            _equip = GetComponent<FPSEquip>();
            
            _equip.OnEquip += OnEquip;
            _equip.OnUnEquip += EquipOnOnUnEquip;
            _equip.OnPlusAttack += Throw;
            _equip.OnPerformAction += Shoot;
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
        
        [ObserversRpc(ExcludeOwner = true)]
        void ThrowObserver()
        {
            Hands.Play(AttackState);
            shootSound.Play();
        }  
        
        [ServerRpc]
        public void ThrowServer(Vector3 pos, Vector2 rot)
        {
            ThrowObserver();
            NetworkObject rocketInstance = Instantiate(rocket, pos, Quaternion.Euler(rot.x, rot.y, 0f));
            Spawn(rocketInstance, Owner);
            Rocket rocketComp = rocketInstance.GetComponent<Rocket>();
            rocketComp.Throw(pos, spearVel, _equip.WeaponHandler.NetworkObject, _equip);
        }

        void Throw()
        {
            Hands.SetTrigger(FPSWeaponHandler.Attack);
        }

        [Client(RequireOwnership = true)]
        void Shoot()
        {
            ThrowServer(Cam.position, Cam.rotation.eulerAngles);
            shootSound.Play();
        }
    }
}
