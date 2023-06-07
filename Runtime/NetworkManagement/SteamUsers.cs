using FishNet.Object;
using FishNet.Object.Synchronizing;
using HeathenEngineering.SteamworksIntegration;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class SteamUsers : NetworkBehaviour
    {
        public static SteamUsers Instance { get; private set; }

        [SyncObject]
        private readonly SyncDictionary<UserData, bool> steamUsers = new();

        [SerializeField] EventObject steamUserJoined;
        [SerializeField] EventObject steamUserLeft;

        [SerializeField] EventObject steamUserReady;
        [SerializeField] EventObject steamUserNotReady;
        [SerializeField] EventObject selfReady;
        [SerializeField] EventObject selfNotReady;
        
        void Awake()
        {
            Instance = this;
            steamUsers.OnChange += SteamUsersOnOnChange;
        }

        void SteamUsersOnOnChange(SyncDictionaryOperation op, UserData key, bool value, bool asserver)
        {
            if (key.IsMe)
            {
                if (value)
                {
                    selfReady.Invoke();
                }
                else
                {
                    selfNotReady.Invoke();
                }
            }
        
            if (value)
            {
                steamUserReady.Invoke();
            }
            else
            {
                steamUserNotReady.Invoke();
            }
        }

        void OnDestroy()
        {
            Instance = null;
        }

        public void SteamUserJoined()
        {
            if(!IsServer) return;
            UserData steamUser = steamUserJoined.ReadValue<UserData>();
            
            Debug.Log($"Adding {steamUser.Nickname} to users in Lobby");
            steamUsers[steamUser] = false;
        }

        public void SteamUserLeft()
        {
            if(!IsServer) return;
            UserData steamUser = steamUserLeft.ReadValue<UserData>();

            Debug.Log($"Removing {steamUser.Nickname} from users in Lobby");
            steamUsers.Remove(steamUser);
        }

        public bool AllReady()
        {
            foreach (var ready in steamUsers.Values)
            {
                if(!ready) return false;
            }

            return true;
        }
        
        public int ReadyUsers()
        {
            int readyUsers = 0;
            foreach (var ready in steamUsers.Values)
            {
                if(!ready) continue;
                readyUsers++;
            }

            return readyUsers;
        }

        [Server]
        public void ToggleReady(UserData user, bool readyState)
        {
            steamUsers[user] = readyState;
        }

        public int TotalUsers => steamUsers.Count;

        public override void OnStopServer()
        {
            base.OnStopServer();
            steamUsers.Clear();
        }
    }
}
