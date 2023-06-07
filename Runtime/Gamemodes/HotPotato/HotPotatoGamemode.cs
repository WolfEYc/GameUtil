using FishNet.Object;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class HotPotatoGamemode : NetworkBehaviour
    {
        [SerializeField] EventObject gameTimeStarted;
        [SerializeField] EventObject gameTimeEnded;
        [SerializeField] NetworkObject potato;
        [SerializeField] float nextPotatoDelay;
        [SerializeField] uint potatoesInGame;

        uint _eatenPotatoes;

        public void StartGame()
        {
            gameTimeStarted.Invoke();
        }
        
        public void SpawnPotato()
        {
            if(!IsServer) return;
            NetworkObject currentKing = NetworkedLeaderboard.instance.CurrentKingNob();
            
            NetworkObject spawnedPotato = Instantiate(potato);
            Debug.Log($"SPAWNING {spawnedPotato} for {currentKing}", currentKing.gameObject);
                
            //GamemodeSwitcher.protectedNobs.Add(spawnedEquip);
            Spawn(spawnedPotato, currentKing.Owner);
            
            FPSEquip equip = spawnedPotato.GetComponent<FPSEquip>();
            equip.Pickup(currentKing);
        }
        
        [ObserversRpc]
        void EndGame()
        {
            gameTimeEnded.Invoke();
        }
        
        public void OnPotatoExplode()
        {
            if(!IsServer) return;
            _eatenPotatoes++;
            
            if (_eatenPotatoes >= potatoesInGame)
            {
                Invoke(nameof(EndGame), 1f);
            }
            else
            {
                Invoke(nameof(SpawnPotato), nextPotatoDelay);
            }
        }
    }
}
