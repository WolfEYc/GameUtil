using UnityEngine;

namespace Wolfey.Events
{
    public class EventInvoker : MonoBehaviour, IEventInvoker
    {
        [SerializeField] Event eventObj;
        
        public void Invoke()
        {
            eventObj.Invoke();
        }

        public void InvokePayload(object data)
        {
            eventObj.Invoke(data);
        }
    }
}
