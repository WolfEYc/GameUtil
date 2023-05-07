using UnityEngine;
using UnityEngine.Serialization;

namespace Wolfey.Events
{
    public class EventInvoker : MonoBehaviour, IEventInvoker
    {
        [FormerlySerializedAs("eventObj")] [SerializeField] EventObject eventObjectObj;
        
        public void Invoke()
        {
            eventObjectObj.Invoke();
        }

        public void InvokePayload(object data)
        {
            eventObjectObj.Invoke(data);
        }
    }
}
