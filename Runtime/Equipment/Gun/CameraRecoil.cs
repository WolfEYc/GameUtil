using UnityEngine;
using Wolfey;

namespace WolfeyFPS
{
    public class CameraRecoil : MonoBehaviour
    {
        [SerializeField] float recoilSpeed;
        [SerializeField] float returnSpeed;

        float _lengthAlongCurve;
        Transform _transform;
        Quaternion _targetRot;

        const float ShotOffsetMult = 2f;

        void Awake()
        {
            _transform = transform;
        }

        public Vector3 Shoot(GunInfo gunInfo)
        {
            float toMove = 1f / gunInfo.magCapacity;
            _lengthAlongCurve += toMove;

            if (_lengthAlongCurve > 1f)
            {
                _lengthAlongCurve = Mathf.PerlinNoise1D(Time.deltaTime * toMove).Remap(0f, 1f, 0.5f, 1f);
            }
            
            Vector3 aimRot = new(
                gunInfo.xRecoil.Evaluate(_lengthAlongCurve),
                gunInfo.yRecoil.Evaluate(_lengthAlongCurve),
                0f);
            
            _targetRot = Quaternion.Euler(aimRot);
            
            return Quaternion.SlerpUnclamped(_transform.parent.rotation, _transform.rotation, ShotOffsetMult) * Vector3.forward;
        }

        void Update()
        {
            var returnDelta = returnSpeed * Time.deltaTime;
            
            _lengthAlongCurve = Mathf.Lerp(_lengthAlongCurve, 0f, returnDelta);
            _targetRot = Quaternion.Slerp(_targetRot, Quaternion.identity, returnDelta);
            
            _transform.localRotation = Quaternion.Slerp(_transform.localRotation, _targetRot, recoilSpeed * Time.deltaTime);
        }
    }
}
