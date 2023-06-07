using System;
using UnityEngine;

namespace WolfeyFPS
{
    [RequireComponent(typeof(SphereCollider))]
    public class GroundCheck : MonoBehaviour
    {
        [SerializeField] LayerMask layerMask;
        [SerializeField] SurfaceSounds defaultFootstepSounds;
        
        FootstepSurface _groundSurface;
        SphereCollider _sphereCollider;
        Transform _transform;

        public event Action GroundedSwitch;

        public bool Grounded { get; private set; }

        Collider _prev;
        Collider[] _results;

        public SurfaceSounds SurfaceSounds =>
            _groundSurface == null ? defaultFootstepSounds : _groundSurface.SurfaceSounds;
        
        void Awake()
        {
            _results = new Collider[1];
            _transform = transform;
            _sphereCollider = GetComponent<SphereCollider>();
        }

        bool IsTriggered()
        {
            int hits = Physics.OverlapSphereNonAlloc(_transform.position, _sphereCollider.radius, _results, layerMask,
                QueryTriggerInteraction.Ignore);
            
            if(hits == 0){
                return false;
            }

            if (ReferenceEquals(_results[0], _prev)) return true;
            _prev = _results[0];
            _groundSurface = _prev.GetComponent<FootstepSurface>();
            return true;
        }

        void FixedUpdate()
        {
            bool triggered = IsTriggered();
            if(Grounded == triggered) return;
            Grounded = triggered;
            GroundedSwitch?.Invoke();
        }
    }
}
