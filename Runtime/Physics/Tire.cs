using DG.Tweening;
using FishNet.Object;
using UnityEngine;
using Wolfey.Audio;

namespace WolfeyFPS
{
    public class Tire : NetworkBehaviour
    {
        [SerializeField] float boingAmt;
        [SerializeField] PlayRandomClip boingSound;
        [SerializeField] ParticleSystem boingParticles;
        [SerializeField] Transform visuals;
        [SerializeField] Vector2 yMinMax;
        
        void OnTriggerEnter(Collider other)
        {
            if(!other.attachedRigidbody.GetComponentInParent<NetworkObject>().IsOwner) return;

            var rb = other.attachedRigidbody;
            var vel = rb.velocity;
            vel.y = Mathf.Clamp(Mathf.Abs(vel.y) + boingAmt, yMinMax.x, yMinMax.y);
            rb.velocity = vel;
            PerformBoing();
            PerformBoingServer();
        }

        [ServerRpc(RequireOwnership = false)]
        void PerformBoingServer()
        {
            PerformBoingObserver();
        }

        [ObserversRpc(ExcludeOwner = true)]
        void PerformBoingObserver()
        {
            PerformBoing();
        }

        void PerformBoing()
        {
            visuals.localPosition = Vector3.zero;
            visuals.DOShakePosition(
                0.5f,
                new Vector3(0f, 0.5f, 0f),
                30).onComplete += OnComplete;
            
            boingSound.Play();
            boingParticles.Play();
        }

        void OnComplete()
        {
            visuals.DOLocalMove(Vector3.zero, 0.25f);
        }
    }
}
