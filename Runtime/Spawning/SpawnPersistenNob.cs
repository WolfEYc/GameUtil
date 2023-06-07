using FishNet;
using FishNet.Object;
using UnityEngine;

namespace WolfeyFPS
{
    public class SpawnPersistenNob : MonoBehaviour
    {
        static NetworkObject _persistentNob;
        
        [SerializeField] NetworkObject persistentNobPrefab;
        
        void Start()
        {
            if(!InstanceFinder.IsServer || _persistentNob != null) return;
            _persistentNob = Instantiate(persistentNobPrefab);
            InstanceFinder.ServerManager.Spawn(_persistentNob);
        }
    }
}
