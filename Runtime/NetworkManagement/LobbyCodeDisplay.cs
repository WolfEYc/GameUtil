using System;
using HeathenEngineering.SteamworksIntegration;
using TMPro;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class LobbyCodeDisplay : MonoBehaviour
    {
        [SerializeField] EventObject lobbyCodeUpdated;
        [SerializeField] TMP_Text lobbyCodeDisplay;
        Lobby _lobby;

        void Start()
        {
            if(lobbyCodeUpdated.ReadValue<object>() == null) return;
            Lobby lobby = lobbyCodeUpdated.ReadValue<Lobby>();
            Display(lobby);
        }

        public void Display(Lobby lobby)
        {
            _lobby = lobby;
            
            lobbyCodeDisplay.SetText(_lobby.LobbyToBase36());
        }

        public void LobbyCodeUpdated()
        {
            Display(lobbyCodeUpdated.ReadValue<Lobby>());
        }
    }
}
