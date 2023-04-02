using UnityEngine;
using UnityEngine.Events;

namespace WolfeyGamedev.Events
{
    public class SerializedEventListener : MonoBehaviour
    {
        [SerializeField] ScriptableEvent scriptableEvent;
        public bool debugPrintEnabled;
        [SerializeField] UnityEvent eventInvoked;
        
        void OnEnable()
        {
            scriptableEvent.Invoked += eventInvoked.Invoke;
            scriptableEvent.Invoked += DebugEvent;
        }
        
        void OnDisable()
        {
            scriptableEvent.Invoked -= eventInvoked.Invoke;
            scriptableEvent.Invoked -= DebugEvent;
        }
        
        void DebugEvent()
        {
            if(!debugPrintEnabled) return;
            Debug.Log(ReadValue<object>().ToString());
        }

        public T ReadValue<T>()
        {
            return scriptableEvent.ReadValue<T>();
        }
    }
}
