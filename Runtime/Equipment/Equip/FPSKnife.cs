using FishNet.Object;
using UnityEngine;

namespace WolfeyFPS
{
    [RequireComponent(typeof(FPSEquip))]
    public class FPSKnife : NetworkBehaviour
    {
        FPSEquip _equip;
        Animator Hands => _equip.WeaponHandler.Hands;
        
        void Awake()
        {
            _equip = GetComponent<FPSEquip>();
            _equip.OnPlusAttack += OnPlusAttack;
        }

        [ObserversRpc(ExcludeOwner = true)]
        void AttackObserver()
        {
            Hands.SetTrigger(FPSWeaponHandler.Attack);
            
        }

        [ServerRpc]
        void AttackServer()
        {
            AttackObserver();
        }
        
        
        void OnPlusAttack()
        {
            AttackServer();
            Hands.SetTrigger(FPSWeaponHandler.Attack);
            
            //Perform knife attack logic here
            
        }
        
    }
}
