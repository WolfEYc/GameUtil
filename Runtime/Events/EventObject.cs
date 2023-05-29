using System;
using UnityEngine;

namespace Wolfey.Events
{
    [CreateAssetMenu(order = 0, menuName = "WolfeyGamedev/ScriptableEvent", fileName = "NewScriptableEvent")]
    public class EventObject : ScriptableObject
    {
        object _value;
    

        public event Action Invoked;
        
        public T ReadValue<T>()
        {
            return (T)_value;
        }
        
        public void Invoke(object value = null)
        {
            _value = value;
            Invoked?.Invoke();
        }
    }
}
