using FishNet.Object;
using UnityEngine;

namespace WolfeyFPS
{
    public class WorldPickup : NetworkBehaviour, IInteractable
    {
        [SerializeField] NetworkObject equipPrefab;
        
        public void Interact(NetworkObject interactor)
        {
            InteractServer(interactor);
        }

        [ServerRpc]
        void InteractServer(NetworkObject interactor)
        {
            NetworkObject spawnedEquip = Instantiate(equipPrefab);
            Spawn(spawnedEquip, interactor.Owner);
            
            FPSEquip equip = spawnedEquip.GetComponent<FPSEquip>();
            equip.Pickup(interactor);
        }
    }
}
