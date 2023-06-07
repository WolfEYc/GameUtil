using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class EventOnDamage : MonoBehaviour, IDamageable
    {
        [SerializeField] EventObject eventToCall;

        public void Damage(float amt, Vector3 pt, Vector3 source, NetworkObject damager, Sprite damageTool)
        {
            eventToCall.Invoke();
        }
    }
}
