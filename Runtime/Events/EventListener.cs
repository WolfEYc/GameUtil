   using UnityEngine;
using UnityEngine.Events;

namespace Wolfey.Events
{
    public class EventListener : MonoBehaviour
    {
        [SerializeField] EventObject eventObj;
        public bool debugPrintEnabled;
        public bool bufferLast;
        public UnityEvent eventInvoked;

        int _calls;
        

        void OnEnable()
        {
            eventObj.Invoked += OnCalled;
            if (bufferLast && _calls != eventObj.Calls)
            {
                OnCalled();
            }
        }
        
        void OnDisable()
        {
            eventObj.Invoked -= OnCalled;
        }
        
        void OnCalled()
        {
            _calls = eventObj.Calls;
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
