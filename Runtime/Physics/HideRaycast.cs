using FishNet.Object;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class HideRaycast : NetworkBehaviour
    {
        [SerializeField] EventObject preRaycastEvent;
        
        int _defaultLayer;

        void Awake()
        {
            _defaultLayer = gameObject.layer;
        }
        
        public void DisplayRaycasts(bool display)
        {
            gameObject.layer = display ? _defaultLayer : Layers.IgnoreRaycastLayer;
        }

        public void OnPreraycast()
        {
            DisplayRaycasts(preRaycastEvent.ReadValue<FPSThrownSpear.PreRaycastData>().Owner != Owner);
        }
    }
}
