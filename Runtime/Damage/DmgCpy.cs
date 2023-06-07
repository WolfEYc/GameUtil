using FishNet.Object;
using UnityEngine;

namespace WolfeyFPS
{
    public class DmgCpy : MonoBehaviour, IDamageable
    {
        [SerializeField] MeshRenderer render;
        [SerializeField] Material hitmat;
        Material _defaultMat;
        
        void Awake()
        {
            _defaultMat = render.material;
        }

        public void Damage(float amt, Vector3 pt, Vector3 source, NetworkObject damager, Sprite damageTool)
        {
            render.material = hitmat;
            CancelInvoke();
            Invoke(nameof(ReturnToNormalMat), 3f);
        }

        void ReturnToNormalMat()
        {
            render.material = _defaultMat;
        }
    }
}
