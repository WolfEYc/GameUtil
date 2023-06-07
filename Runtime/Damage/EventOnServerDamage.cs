using FishNet.Object;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class EventOnServerDamage : NetworkBehaviour, IDamageable
    {
        [SerializeField] EventObject eventToCall;

        public void Damage(float amt, Vector3 pt, Vector3 source, NetworkObject damager, Sprite damageTool)
        {
            if(!damager.Owner.IsHost) return;
            eventToCall.Invoke();
            CallOnClients();
        }

        [ObserversRpc]
        void CallOnClients()
        {
            eventToCall.Invoke();
        }
    }
}
