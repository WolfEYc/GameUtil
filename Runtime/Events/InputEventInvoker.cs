using UnityEngine;
using UnityEngine.InputSystem;

namespace Wolfey.Events
{
    public class InputEventInvoker : SerializedEventInvoker
    {
        [SerializeField] InputAction action;
        
        void OnEnable()
        {
            action.performed += ActionOnperformed;
            action.Enable();
        }

        void OnDisable()
        {
            action.Disable();
            action.performed -= ActionOnperformed;
        }

        void ActionOnperformed(InputAction.CallbackContext obj)
        {
            Invoke();
        }
    }
}
