using UnityEngine;

namespace WolfeyFPS
{
    public class LadderTrigger : MonoBehaviour
    {
        [SerializeField] AudioSource climbSound;
        
        void OnTriggerEnter(Collider other)
        {
            var controller = other.attachedRigidbody.GetComponent<FPSController>();
            if(!controller.IsOwner) return;
            controller.LadderClimbing = true;
            climbSound.Play();
        }

        void OnTriggerExit(Collider other)
        {
            var controller = other.attachedRigidbody.GetComponent<FPSController>();
            if(!controller.IsOwner) return;
            controller.LadderClimbing = false;
            climbSound.Stop();
        }
    }
}
