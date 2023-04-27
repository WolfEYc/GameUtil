using UnityEngine;
using UnityEngine.Events;

namespace Wolfey.Events
{
    public class EventListener : MonoBehaviour
    {
        [SerializeField] Event eventObj;
        public bool debugPrintEnabled;
        [SerializeField] UnityEvent eventInvoked;
        
        void OnEnable()
        {
            eventObj.Invoked += eventInvoked.Invoke;
            eventObj.Invoked += DebugEventObj;
        }
        
        void OnDisable()
        {
            eventObj.Invoked -= eventInvoked.Invoke;
            eventObj.Invoked -= DebugEventObj;
        }
        
        void DebugEventObj()
        {
            if(!debugPrintEnabled) return;
            Debug.Log(ReadValue<object>().ToString());
        }

        public T ReadValue<T>()
        {
            return eventObj.ReadValue<T>();
        }
    }
}
