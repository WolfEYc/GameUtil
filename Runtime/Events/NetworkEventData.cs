using System;
using FishNet.Connection;

[Serializable]
public class NetworkEventData
{
    public NetworkConnection Owner;

    public NetworkEventData(NetworkConnection owner)
    {
        Owner = owner;
    }
}
