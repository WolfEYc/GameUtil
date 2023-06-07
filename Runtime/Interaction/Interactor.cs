using FishNet.Object;
using UnityEngine;

namespace WolfeyFPS
{
    public class Interactor : NetworkBehaviour
    {
        [SerializeField] LayerMask interactableLayer;
        [SerializeField] float interactionDist;
        
        public void PerformInteraction()
        {
            if(!Physics.Raycast(
                   transform.position,
                   transform.forward,
                   out RaycastHit hit,
                   interactionDist,
                   interactableLayer,
                   QueryTriggerInteraction.Collide)) return;
            
            hit.collider.GetComponent<IInteractable>().Interact(NetworkObject);
        }
    }
}
