using FishNet;
using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Transporting;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using UnityEngine;
using Wolfey.Events;
using Wolfey.Systems;

namespace WolfeyFPS
{
    public class AutoStartArena : PersistentSingleton<AutoStartArena>
    {
        NetworkObject _persistNob;
        
        ServerManager _serverManager;
        ClientManager _clientManager;
        
        [SerializeField] LobbyManager lobbyManager;
        [SerializeField] NetworkObject persistentNob;
        [SerializeField] EventObject lobbyJoinReq;
        [SerializeField] EventObject presentLobbyUpdated;

        void Start()
        {
            if (lobbyManager.HasLobby)
            {
                Debug.Log("Leaving Existing Lobby:((");
                lobbyManager.Leave();
            }
            
            _serverManager = InstanceFinder.ServerManager;
            _clientManager = InstanceFinder.ClientManager;
            
            _serverManager.OnServerConnectionState += ServerManagerOnOnServerConnectionState;

            Application.quitting += LeaveLobby;
        }

        void ServerManagerOnOnServerConnectionState(ServerConnectionStateArgs obj)
        {
            switch (obj.ConnectionState)
            {
                case LocalConnectionState.Started:
                    if (_persistNob != null)
                    {
                        Destroy(_persistNob);
                    }

                    _persistNob = Instantiate(persistentNob);
                    _serverManager.Spawn(_persistNob);
                    break;
            }
        }


        public void StartHost()
        {
            Debug.Log("Starting Host");
            if(lobbyManager.HasLobby)
            {
                Debug.Log("I HAVE A LOBBY :((");
                return;
            }
            
            lobbyManager.Create();
        }

        public void OnLobbyJoined(Lobby lobby)
        {
            Debug.Log("Lobby Joined!");

            if (_clientManager.Started)
            {
                _clientManager.StopConnection();
            }

            if (_serverManager.Started)
            {
                _serverManager.StopConnection(true);
            }
            
            presentLobbyUpdated.Invoke(lobby);
            _clientManager.StartConnection(lobby.Owner.user.id.ToString());
        }

        public void LeaveLobby()
        {
            Application.quitting -= LeaveLobby;
            
            if(!lobbyManager.HasLobby) return;
            lobbyManager.Leave();
        }

        public void OnCreateLobby(Lobby lobby)
        {
            _serverManager.StartConnection();
            _clientManager.StartConnection();
            presentLobbyUpdated.Invoke(lobby);
        }

        public void OnLeaveLobby()
        {
            if (_clientManager.Started)
            {
                _clientManager.StopConnection();
            }
            
            if (_serverManager.Started)
            {
                _serverManager.StopConnection(true);
            }
        }

        public void GameLobbyJoinRequested(GameLobbyJoinRequested_t joinrequest)
        {
            if (lobbyManager.HasLobby)
            {
                lobbyManager.Leave();
            }

            lobbyManager.Join(joinrequest.m_steamIDLobby);
        }

        public void LobbyJoinReq()
        {
            Lobby lobby = lobbyJoinReq.ReadValue<Lobby>();

            if (lobbyManager.HasLobby)
            {
                lobbyManager.Leave();
            }
            
            lobbyManager.Join(lobby);
        }

        public void LockDownLobby()
        {
            if(!InstanceFinder.NetworkManager.IsServer) return;
            lobbyManager.SetJoinable(false);
        }

        public void OpenUpLobby()
        {
            if(!InstanceFinder.NetworkManager.IsServer) return;
            lobbyManager.SetJoinable(true);
        }
        
        
    }
}
