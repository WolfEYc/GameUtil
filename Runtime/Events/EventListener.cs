   using UnityEngine;
using UnityEngine.Events;

namespace Wolfey.Events
{
    public class EventListener : MonoBehaviour
    {
        [SerializeField] EventObject eventObj;
        public bool debugPrintEnabled;
        public UnityEvent eventInvoked;

        
        void OnEnable()
        {
            eventObj.Invoked += OnCalled;
        }
        
        void OnDisable()
        {
            eventObj.Invoked -= OnCalled;
        }
        
        void OnCalled()
        {
            eventInvoked.Invoke();
            if(!debugPrintEnabled) return;
            Debug.Log(ReadValue<object>().ToString());
        }

        public T ReadValue<T>()
        {
            return eventObj.ReadValue<T>();
        }
    }
}
