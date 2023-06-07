using FishNet.Object;
using UnityEngine;
using WolfeyFPS;

namespace SpearMonkey
{
    public class EquipmentForcePickup : NetworkBehaviour
    {
        [SerializeField] FPSWeaponHandler weaponHandler;
        
        public void ForceEquipment()
        {
            if(!IsServer || weaponHandler.HasEquipment()) return;
            
            NetworkObject[] equipment = PlayerEquipment.Instance.equipment;
            
            for(int i = 0; i < equipment.Length; i++)
            {
                NetworkObject spawnedEquip = Instantiate(equipment[i]);
                Debug.Log($"SPAWNING equip {i}: {spawnedEquip} for {Owner}");
                
                //GamemodeSwitcher.protectedNobs.Add(spawnedEquip);
                Spawn(spawnedEquip, Owner);
                
                FPSEquip equip = spawnedEquip.GetComponent<FPSEquip>();
                equip.Pickup(NetworkObject);
            }
        }
    }
}
