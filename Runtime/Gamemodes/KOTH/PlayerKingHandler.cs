using HeathenEngineering.SteamworksIntegration;
using SpearMonkey;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class PlayerKingHandler : MonoBehaviour
    {
        [SerializeField] EventObject kingChange;
        [SerializeField] PlayerAudio playerSounds;
        [SerializeField] AudioClip gainKing, loseKing;
        [SerializeField] GameObject crown;
        [SerializeField] CosmeticSlot hatSlot;
        [SerializeField] NetworkedSteamUser steamUser;
        [SerializeField] GameObject bodyRenderer;
        
        bool _selfKing;
        
        
        public void OnKingChange()
        {
            if (ReferenceEquals(kingChange.ReadValue<object>(), null) || kingChange.ReadValue<UserData>() != steamUser.SteamUser)
            {
                LoseKing();
                return;
            }
            
            GainKing();
        }

        void LoseKing()
        {
            if(!_selfKing) return; 
            _selfKing = false;
            playerSounds.Play(loseKing);
            hatSlot.EquipDefault();
            bodyRenderer.layer = Layers.PlayerLayer;
        }

        void GainKing()
        {
            if(_selfKing) return;
            _selfKing = true;
            playerSounds.Play(gainKing);
            hatSlot.Equip(crown);
            bodyRenderer.layer = Layers.KingLayer;
        }
    }
}
