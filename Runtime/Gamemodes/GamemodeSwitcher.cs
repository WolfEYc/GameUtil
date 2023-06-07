using System.Collections.Generic;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;
using Wolfey;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class GamemodeSwitcher : NetworkBehaviour
    {
        [SerializeField] EventObject newGameStarted, gameModeStarted, newSpawnRequested;

        [Tooltip("least to most intense")]
        [SerializeField] string[] easyGamemodes;
        [SerializeField] string[] medGamemodes;
        [SerializeField] string[] hardGamemodes;
        
        [SerializeField] string startScene;
        [SerializeField] string gameOverScene;
        
        public int gamemodesInPlayThrough;
        public int CompletedLevels { get; private set; }

        const float LOAD_DELAY = 2f;
        
        const float OneThird = 1f / 3f;
        const float TwoThird = 2f / 3f;

        string _currentServerScene;
        bool _loadingNewScene;
        bool _waitingForReady;
        bool _initialized;

        public static List<NetworkObject> protectedNobs;
        

        void Awake()
        {
            protectedNobs = new List<NetworkObject>();
            _currentServerScene = startScene;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            if(_initialized) return;
            _initialized = true;
            SceneManager.OnClientPresenceChangeEnd += SceneManagerOnOnClientPresenceChangeEnd;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            _initialized = false;
            SceneManager.OnClientPresenceChangeEnd -= SceneManagerOnOnClientPresenceChangeEnd;
        }
        
        public void SceneManagerOnOnClientPresenceChangeEnd(ClientPresenceChangeEventArgs obj)
        {
            if(!_loadingNewScene || obj.Scene.name != _currentServerScene) return;
            if (!SceneManager.SceneConnections.TryGetValue(SceneManager.GetScene(_currentServerScene), out var value))
            {
                return;
            }
            
            if(ServerManager.Clients.Count != value.Count) return;
            
            //Everybody is loaded??
            _loadingNewScene = false;
            NewSpawnRequest();

            if (CompletedLevels == 0 || CompletedLevels == gamemodesInPlayThrough)
            {
                GameModeStarted();
            }
            else
            {
                _waitingForReady = true;
            }
        }
        
        public void ReadyUpsChanged()
        {
            if(!IsServer || !_waitingForReady || !SteamUsers.Instance.AllReady()) return;
            _waitingForReady = false;
            GameModeStarted();
        }
        
        [ObserversRpc]
        void NewSpawnRequest()
        {
            newSpawnRequested.Invoke();
        }

        [ObserversRpc]
        void GameModeStarted()
        {
            gameModeStarted.Invoke();
        }

        SceneLoadData GetSceneLoadData()
        {
            return new SceneLoadData(_currentServerScene)
            {
                MovedNetworkObjects = protectedNobs.ToArray(),
                ReplaceScenes = ReplaceOption.All
            };
        }

        void SetSceneToLoad()
        {
            if (CompletedLevels == 0)
            {
                _currentServerScene = startScene;
                return;
            }
            
            float completion = (float)CompletedLevels / (gamemodesInPlayThrough + 1);

            _currentServerScene = completion switch
            {
                < 0.01f => startScene,
                < OneThird => easyGamemodes.RandomElement(),
                < TwoThird => medGamemodes.RandomElement(),
                < .99f => hardGamemodes.RandomElement(),
                _ => gameOverScene
            };
        }
        void LoadNextGamemode()
        {
            if(!IsServer) return;

            _loadingNewScene = true;
            SetSceneToLoad();
            SceneLoadData sld = GetSceneLoadData();
            SceneManager.LoadGlobalScenes(sld);
        }

        public void OnGamemodeEnded()
        {
            if (CompletedLevels == 0)
            {
                newGameStarted.Invoke();
            }
            
            if (CompletedLevels > gamemodesInPlayThrough)
            {
                CompletedLevels = 0;
            }
            else
            {
                CompletedLevels++;
            }
        }

        public void LoadNextGameMode()
        {
            if(!IsServer) return;
            Invoke(nameof(LoadNextGamemode), LOAD_DELAY);
        }
    }
}
