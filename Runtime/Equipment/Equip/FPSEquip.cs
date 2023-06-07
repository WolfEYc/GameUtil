using System;
using FishNet.Object;
using UnityEngine;
using Wolfey;


namespace WolfeyFPS
{
    public class FPSEquip : NetworkBehaviour
    {
        [SerializeField] string intendedName;
        [SerializeField] AnimatorOverrideController overrideController;
        [SerializeField] AnimatorOverrideController tpsOverrideController;

        [field:SerializeField] public int EquipIdx { get; private set; }
        [field:SerializeField] public GameObject Visuals { get; private set; }
        [field:SerializeField] public Sprite Icon { get; private set; }

        public FPSWeaponHandler WeaponHandler { get; private set; }
        
        public event Action OnEquip,
            OnPickup,
            OnSwitchReady,
            OnUnEquip,
            OnPlusAttack,
            OnMinusAttack,
            OnReload,
            OnReloadFinished,
            OnPerformAction;

        public bool Equipped => !ReferenceEquals(WeaponHandler, null) && WeaponHandler.CurrentEquip == this;
        public int Layer => _owningPlayer.Owner.IsLocalClient ? Layers.FPSModelsLayer : Layers.PlayerLayer;
  
        NetworkObject _owningPlayer;

        void Awake()
        {
            gameObject.name = intendedName;
        }
        
        [ObserversRpc(BufferLast = true)]
        public void Pickup(NetworkObject owner)
        {
            if(_owningPlayer == owner) return;

            if (WeaponHandler != null)
            {
                WeaponHandler.LoseEquip(EquipIdx);
            }
            
            NetworkObject.GiveOwnership(owner.Owner);
            _owningPlayer = owner;
            Debug.Log($"PERFORMING PICKUP For", _owningPlayer.gameObject);
            
            WeaponHandler = _owningPlayer.GetComponent<FPSWeaponHandler>();
            gameObject.SetLayerRecursively(Layer);
            var transform1 = transform;
            transform1.parent = WeaponHandler.WeaponHoldTransform;
            transform1.localScale = Vector3.one;
            transform1.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            WeaponHandler.PickupObserver(this);
            OnPickup?.Invoke();
        }

        public void Equip()
        {
            WeaponHandler.controller.maxSpeedMult = 1f;
            Visuals.SetActive(true);
            WeaponHandler.Hands.runtimeAnimatorController = _owningPlayer.Owner.IsLocalClient ? overrideController : tpsOverrideController;
            OnEquip?.Invoke();
        }

        public void UnEquip()
        {
            Visuals.SetActive(false);
            OnUnEquip?.Invoke();
        }

        public void SwitchReady()
        {
            OnSwitchReady?.Invoke();
        }

        public void PlusAttack()
        {
            OnPlusAttack?.Invoke();
        }

        public void MinusAttack()
        {
            OnMinusAttack?.Invoke();
        }

        public void Reload()
        {
            OnReload?.Invoke();
        }

        public void ReloadFinished()
        {
            OnReloadFinished?.Invoke();
        }

        public void PerformAction()
        {
            OnPerformAction?.Invoke();
        }

        public void PerformDitch()
        {
            WeaponHandler.DitchEquip(EquipIdx);
        }
    }
}
