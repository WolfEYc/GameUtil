using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace WolfeyFPS
{
    public class InputActionListener : MonoBehaviour
    {
        [SerializeField] InputAction inputAction;
        public UnityEvent actionPerformed;
        
        void Awake()
        {
            inputAction.performed += InputActionOnperformed;
        }

        void InputActionOnperformed(InputAction.CallbackContext obj)
        {
            if (obj.performed)
            {
                actionPerformed.Invoke();
            }
        }


        void OnEnable()
        {
            inputAction.Enable();
        }

        void OnDisable()
        {
            inputAction.Disable();
        }
    }
}
