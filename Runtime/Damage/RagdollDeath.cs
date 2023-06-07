using Cinemachine;
using FishNet.Object;
using UnityEngine;
using Wolfey.Events;
using Extensions = Wolfey.Extensions;

namespace WolfeyFPS
{
    public class RagdollDeath : NetworkBehaviour
    {
        [SerializeField] FPSController controller;
        [SerializeField] GameObject ragdollRoot;
        [SerializeField] Transform ragdollHips;
        [SerializeField] CinemachineVirtualCamera ragdollCam;
        [SerializeField] EventObject deathEvent, spawnEvent;
        [SerializeField] SkinnedMeshRenderer tpsRenderer;
        [SerializeField] float angularMult;
        
        Rigidbody[] _ragDollRbs;

        void Awake()
        {
            _ragDollRbs = ragdollHips.GetComponentsInChildren<Rigidbody>();
        }

        public void OnDeath()
        {
            DeathData deathData = deathEvent.ReadValue<DeathData>();
            var position = deathData.PhysicalTransform.position;

            for (int i = 0; i < _ragDollRbs.Length; i++)
            {
                _ragDollRbs[i].isKinematic = false;
                _ragDollRbs[i].velocity = controller.Rb.velocity;
                _ragDollRbs[i].angularVelocity = Extensions.RandomV3() * angularMult;
            }

            ragdollRoot.SetActive(true);
            ragdollHips.SetPositionAndRotation(position, deathData.PhysicalTransform.rotation);

            tpsRenderer.enabled = false;

            if (!IsOwner)
            {
                return;
            }

            ragdollCam.Priority = 20;
            controller.enabled = false;
        }

        public void OnSpawn()
        {
            ragdollRoot.SetActive(false);

            for (int i = 0; i < _ragDollRbs.Length; i++)
            {
                _ragDollRbs[i].isKinematic = true;
            }

            if (!IsOwner)
            {
                return;
            }

            ragdollCam.Priority = 0;
            controller.enabled = true;
            ragdollCam.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
