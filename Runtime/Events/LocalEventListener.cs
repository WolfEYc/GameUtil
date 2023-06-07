using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;
using Wolfey.Events;

/// <summary>
/// This event will only be called if the owner of the object also owns the event
/// </summary>

namespace WolfeyFPS
{
    public class LocalEventListener : NetworkBehaviour
    {
        [SerializeField] EventObject eventObject;
        [SerializeField] UnityEvent eventOccured;

        void OnEnable()
        {
            eventObject.Invoked += EventObjectOnInvoked;
        }

        void OnDisable()
        {
            eventObject.Invoked -= EventObjectOnInvoked;
        }

        void EventObjectOnInvoked()
        {
            if (!Owner.IsLocalClient) return;
            eventOccured.Invoke();
        }
    }
}
