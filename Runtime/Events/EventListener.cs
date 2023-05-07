using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Wolfey.Events
{
    public class EventListener : MonoBehaviour
    {
        [FormerlySerializedAs("eventObj")] [SerializeField] EventObject eventObjectObj;
        public bool debugPrintEnabled;
        [SerializeField] UnityEvent eventInvoked;
        
        void OnEnable()
        {
            eventObjectObj.Invoked += eventInvoked.Invoke;
            eventObjectObj.Invoked += DebugEventObjectObj;
        }
        
        void OnDisable()
        {
            eventObjectObj.Invoked -= eventInvoked.Invoke;
            eventObjectObj.Invoked -= DebugEventObjectObj;
        }
        
        void DebugEventObjectObj()
        {
            if(!debugPrintEnabled) return;
            Debug.Log(ReadValue<object>().ToString());
        }

        public T ReadValue<T>()
        {
            return eventObjectObj.ReadValue<T>();
        }
    }
}
