using UnityEngine;
using UnityEngine.Events;

namespace Wolfey
{
    public class Toggleable : MonoBehaviour
    {
        bool _on;

        public UnityEvent turnedOn;
        public UnityEvent turnedOff;
        
        public void Toggle(bool on)
        {
            if(_on == on) return;
            _on = on;

            if (_on)
            {
                turnedOn.Invoke();
            }
            else
            {
                turnedOff.Invoke();
            }
        }

        public void Flip()
        {
            Toggle(!_on);
        }
    }
}
