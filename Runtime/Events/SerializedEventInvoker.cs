using UnityEngine;

namespace WolfeyGamedev.Events
{
    public class SerializedEventInvoker : MonoBehaviour, IEventInvoker
    {
        [SerializeField] ScriptableEvent scriptableEvent;
        
        public void Invoke()
        {
            scriptableEvent.Invoke();
        }

        public void InvokePayload(object data)
        {
            scriptableEvent.Invoke(data);
        }
    }
}
