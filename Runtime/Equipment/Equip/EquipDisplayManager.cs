using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class EquipDisplayManager : MonoBehaviour
    {
        [SerializeField] EventObject equipChanged;
        [SerializeField] EventObject equipIdxChanged;
        [SerializeField] EquipDisplay[] equipDisplays;
        
        public void OnEquipChange()
        {
            EquipEventData equipEventData = equipChanged.ReadValue<EquipEventData>();
            equipDisplays[equipEventData.idx].SetEquip(equipEventData.equip);
        }

        public void OnEquipIdxChange()
        {
            FPSWeaponHandler.EquipIdxData equipIdxData = equipIdxChanged.ReadValue<FPSWeaponHandler.EquipIdxData>();

            foreach (EquipDisplay equipDisplay in equipDisplays)
            {
                equipDisplay.UnEquip();
            }
            
            equipDisplays[equipIdxData.idx].Equip();
        }
    }
}
