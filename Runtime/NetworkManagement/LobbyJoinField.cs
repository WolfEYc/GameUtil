using DG.Tweening;
using HeathenEngineering.SteamworksIntegration;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class LobbyJoinField : MonoBehaviour
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] EventObject joinLobbyReq;
        [SerializeField] UnityEvent setBtnInteractable;
        [SerializeField] UnityEvent setBtnNONInteractable;

        void Awake()
        {
            inputField.onValidateInput += (_, _, addedChar) => char.ToUpper(addedChar);
        }

        public void Submit()
        {
            setBtnNONInteractable.Invoke();
            Invoke(nameof(SetInteractableDelay), 3f);
            Lobby lobby = inputField.text.Base36ToLobby();
            
            if (!lobby.IsValid)
            {
                JoinFailed();
                return;
            }
            
            joinLobbyReq.Invoke(lobby);
        }

        public void JoinFailed()
        {
            UIAudio.PlayInputFailedAudio();
            transform.DOShakePosition(0.1f, 20f, 50);
            setBtnInteractable.Invoke();
        }

        void SetInteractableDelay()
        {
            setBtnInteractable.Invoke();
        }
    }
}
