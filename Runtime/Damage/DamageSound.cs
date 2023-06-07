using FishNet.Object;
using UnityEngine;
using Wolfey.Audio;

namespace WolfeyFPS
{
    public class DamageSound : NetworkBehaviour, IDamageable
    {
        [SerializeField] PlayRandomClip playOnHit;

        public void Damage(float amt, Vector3 pt, Vector3 source, NetworkObject damager, Sprite damageTool)
        {
            playOnHit.Play();
        }
    }
}
