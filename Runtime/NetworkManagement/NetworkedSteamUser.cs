using System;
using System.Collections.Generic;
using FishNet.Object;

using HeathenEngineering.SteamworksIntegration;
using UnityEngine;
using Wolfey.Events;
using WolfeyFPS;

public class NetworkedSteamUser : NetworkBehaviour
{
    public static Dictionary<UserData, NetworkObject> Usermap { get; private set; }


    [SerializeField] EventObject steamUserJoined;
    [SerializeField] EventObject steamUserLeft;
    
    
    public UserData SteamUser { get; private set; }

    [ObserversRpc(BufferLast = true, ExcludeOwner = true, ExcludeServer = true)]
    void SetSteamUserObservers(UserData data)
    {
        SteamUser = data;
        Usermap[SteamUser] = NetworkObject;
        steamUserJoined.Invoke(data);
    }
    
    [ServerRpc]
    void SetSteamUserServer(UserData data)
    {
        SetSteamUserObservers(data);
        SteamUser = data;
        Usermap[SteamUser] = NetworkObject;
        steamUserJoined.Invoke(data);
    }

    void Awake()
    {
        Usermap ??= new Dictionary<UserData, NetworkObject>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(!IsOwner) return;
        
        if (!SteamSettings.Initialized)
        {
            SteamSettings.behaviour.evtSteamInitialized.AddListener(NetworkTheUser);
            return;
        }
        
        NetworkTheUser();
    }

    [Client(RequireOwnership = true)]
    void NetworkTheUser()
    {
        SetSteamUserServer(UserData.Me);

        if (IsServer) return;
        
        SteamUser = UserData.Me;
        Usermap[SteamUser] = NetworkObject;
        steamUserJoined.Invoke(SteamUser);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        steamUserLeft.Invoke(SteamUser);
    }
    
    public void ToggleReady(bool ready)
    {
        if(!IsOwner) return;
        ToggleReadyServer(ready);
    }

    [ServerRpc]
    void ToggleReadyServer(bool ready)
    {
        SteamUsers.Instance.ToggleReady(SteamUser, ready);
    }
}
