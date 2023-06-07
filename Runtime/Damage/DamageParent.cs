using FishNet.Object;
using UnityEngine;

namespace WolfeyFPS
{
    public class DamageParent : MonoBehaviour
    {
        public Transform damageTransform;
        public Transform head;
        IDamageable[] _damageables;

        void Awake()
        {
            _damageables = GetComponentsInChildren<IDamageable>();
        }

        public void DamageAll(float amt, Vector3 pt, Vector3 source, NetworkObject damager, Sprite damageTool)
        {
            for (int i = 0; i < _damageables.Length; i++)
            {
                _damageables[i].Damage(amt, pt, source, damager, damageTool);
            }
        }
    }
}
