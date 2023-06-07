using FishNet.Connection;

namespace WolfeyFPS
{
    public class EquipEventData : NetworkEventData
    {
        public int idx;
        public FPSEquip equip;

        public EquipEventData(int idx, FPSEquip equip, NetworkConnection owner) : base(owner)
        {
            this.idx = idx;
            this.equip = equip;
        }
    }
}
