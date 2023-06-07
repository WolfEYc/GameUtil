using UnityEngine;
using UnityEngine.UI;

namespace WolfeyFPS
{
    public class EquipDisplay : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] Image bg;
        [SerializeField] Color unEquippedIcon;
        [SerializeField] Color equippedBg;
        [SerializeField] Color unEquippedBg;
        
        bool _equipped;
        
        public void SetEquip(FPSEquip equip)
        {
            if (ReferenceEquals(equip, null))
            {
                icon.enabled = false;
                bg.enabled = _equipped;
            }
            else
            {
                icon.sprite = equip.Icon;
                icon.enabled = true;
                bg.enabled = true;
            }
        }

        public void Equip()
        {
            _equipped = true;
            bg.enabled = true;
            icon.color = Color.white;
            bg.color = equippedBg;
        }

        public void UnEquip()
        {
            _equipped = false;
            icon.color = unEquippedIcon;
            bg.color = unEquippedBg;
            bg.enabled = icon.enabled;
        }
    }
}
