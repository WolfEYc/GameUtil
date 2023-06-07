using System.Collections.Generic;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using UnityEngine;
using Wolfey.Systems;

namespace SpearMonkey
{
    public class CostmeticLookupSystem : Singleton<CostmeticLookupSystem>
    {
        [SerializeField] ItemDefinition[] steamItemIds;
        [SerializeField] GameObject[] mappedPrfbs;

        Dictionary<SteamItemDef_t, GameObject> _itemMappings;
        public static Dictionary<SteamItemDef_t, GameObject> ItemMappings => _instance._itemMappings;

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < steamItemIds.Length; i++)
            {
                _itemMappings[steamItemIds[i].Id] = mappedPrfbs[i];
            }
        }
    }
}
