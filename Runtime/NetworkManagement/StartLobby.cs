using FishNet.Object;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class StartLobby : NetworkBehaviour
    {
        [SerializeField] EventObject startLobbyActive;
        
        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            startLobbyActive.Invoke();
        }
    }
}
