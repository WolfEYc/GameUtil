using FishNet.Object;
using HeathenEngineering.SteamworksIntegration;
using UnityEngine;
using Wolfey;
using WolfeyFPS;

namespace SpearMonkey
{
    public class CosmeticSlot : NetworkBehaviour
    {
        [SerializeField] bool thirdPersonOnly;
        
        ItemDetail _default;
        bool _hasDefault;
        bool _hasEquipped;

        void Start()
        {
            TryRefreshDefault();
        }

        public void TryRefreshDefault()
        {
            string b36id = PlayerPrefs.GetString(gameObject.name, "");
            
            if(b36id == string.Empty 
               || !CosmeticsManager.InventoryItems.TryGetValue(
                   b36id.B36ToULong(),
                   out _default)) return;
            
            _hasDefault = true;
            if(_hasEquipped) return;
            EquipDefault();
        }

        public void EquipDefault()
        {
            if (!_hasDefault)
            {
                transform.DestroyChildren();
                _hasEquipped = false;
                return;
            }
            
            Equip(_default);
        }

        public void SetDefaultNone()
        {
            _hasDefault = false;
            PlayerPrefs.SetString(gameObject.name, "");
        }
        
        public void SetDefault(ItemDetail item)
        {
            _hasDefault = true;
            _default = item;
            PlayerPrefs.SetString(gameObject.name, _default.ItemId.m_SteamItemInstanceID.ULongToB36());
        }
        
        public void Equip(ItemDetail item)
        {
            Equip(CostmeticLookupSystem.ItemMappings[item.Definition]);
        }

        public void Equip(GameObject prfb)
        {
            _hasEquipped = true;
            Transform transform1 = transform;
            
            transform1.DestroyChildren();
            Instantiate(prfb, transform1).SetActive(!thirdPersonOnly || !Owner.IsLocalClient);
        }
    }
}
 