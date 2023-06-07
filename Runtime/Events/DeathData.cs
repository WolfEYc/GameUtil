using FishNet.Object;
using UnityEngine;
using WolfeyFPS;

public class DeathData : NetworkEventData
{
    public NetworkObject DeadObject;
    public Transform PhysicalTransform;
    public Sprite killTool;
    public NetworkObject Killer;

    public DeathData(NetworkObject deadObject, Transform physicalTransform) : base(deadObject.Owner)
    {
        DeadObject = deadObject;
        PhysicalTransform = physicalTransform;
        Killer = null;
    }
}