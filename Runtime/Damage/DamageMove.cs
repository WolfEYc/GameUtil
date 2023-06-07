using FishNet.Object;
using UnityEngine;

namespace WolfeyFPS
{
    [RequireComponent(typeof(Rigidbody))]
    public class DamageMove : MonoBehaviour, IDamageable
    {
        [SerializeField] FPSController controller;
        Rigidbody _rb;

        const float Dmg2Force = 0.75f;
        
        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void Damage(float amt, Vector3 pt, Vector3 source, NetworkObject obj, Sprite damageTool)
        {
            float force = amt * Dmg2Force;
            Vector3 pushDir = source - pt;
            pushDir.Normalize();
            controller.hitFricMult = 0f;
            _rb.AddForce(pushDir * force, ForceMode.Impulse);
        }
    }
}
