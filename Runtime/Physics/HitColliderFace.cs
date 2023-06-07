using FishNet.Object;
using UnityEngine;
using Wolfey.Events;
using WolfeyFPS;

[RequireComponent(typeof(Collider))]
public class HitColliderFace : NetworkBehaviour
{
    [SerializeField] EventObject preRayCast;
    
    Transform _transform;

    const int IgnoreRaycast = 2;

    void Awake()
    {
        _transform = transform;
    }

    [Server]
    public void LookAtCaster()
    {
        FPSThrownSpear.PreRaycastData raycastData = preRayCast.ReadValue<FPSThrownSpear.PreRaycastData>();
        
        gameObject.layer = Owner != raycastData.Owner ? 0 : IgnoreRaycast;
        
        _transform.LookAt(raycastData.Pos, Vector3.up);
    }
}
