using System.Collections.Generic;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using Steamworks;
using UnityEngine;
using Wolfey.Events;
using Wolfey.Systems;

namespace WolfeyFPS
{
    public class CosmeticsManager : Singleton<CosmeticsManager>
    {
        Dictionary<ulong, ItemDetail> _inventoryItems;
        public static Dictionary<ulong, ItemDetail> InventoryItems => _instance._inventoryItems;
        [SerializeField] EventObject inventoryUpdated;

        protected override void Awake()
        {
            base.Awake();
            _inventoryItems = new Dictionary<ulong, ItemDetail>();
        }

        public void ForceUpdateInventory()
        {
            Inventory.Client.GetAllItems(OnInventoryRetrieved);
        }

        public void HandleInventoryUpdate(InventoryChangeRecord inventoryRecord)
        {
            ForceUpdateInventory();
        }

        void OnInventoryRetrieved(InventoryResult inventoryResult)
        {
            if (inventoryResult.result != EResult.k_EResultOK)
            {
                Debug.Log("Error fetching inventory");
                return;
            }
            
            _inventoryItems.Clear();
            
            foreach (var itemdetail in inventoryResult.items)
            {
                _inventoryItems[itemdetail.ItemId.m_SteamItemInstanceID] = itemdetail;
            }
            
            inventoryUpdated.Invoke();
        }
    }
}
