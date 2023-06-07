using FishNet.Object;
using UnityEngine;

namespace WolfeyFPS
{
    public interface IDamageable
    {
        void Damage(float amt, Vector3 pt, Vector3 source, NetworkObject damager, Sprite damageTool);
    }
}
