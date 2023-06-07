using System.Collections;
using FishNet.Object;
using HeathenEngineering.SteamworksIntegration;
using UnityEngine;
using Wolfey.Audio;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class KOTHGamemode : NetworkBehaviour
    {
        [SerializeField] EventObject requestChangeScore;
        [SerializeField] float ptsPerSec;
        [SerializeField] float halfBox;
        [SerializeField] LayerMask playerMask;
        [SerializeField] EventObject kingChanged;
        [SerializeField] PlayRandomClip kingRewardedSound;
        [SerializeField] GameObject crownOnPedestal;
        
        Quaternion _rotation;
        Vector3 _halfBox;
        Vector3 _center;
        UserData _kingData;
        Collider _king;
        
        Collider[] _results;
        const uint MaxPlayers = 4;

        WaitForSeconds _waitForScan;

        public override void OnStartServer()
        {
            base.OnStartServer();
            StartCoroutine(ScanRoutine());
        }

        IEnumerator ScanRoutine()
        {
            _results = new Collider[MaxPlayers];
            _halfBox = Vector3.one * halfBox;
            var transform1 = transform;
            _center = transform1.position;
            _rotation = transform1.rotation;
            _waitForScan = new WaitForSeconds(1f);
            
            while (true)
            {
                yield return _waitForScan;
                PerformScan();
            }
        }

        public void ChangeScanRate(float newScanRate)
        {
            _waitForScan = new WaitForSeconds(newScanRate);
        }

        public void TimeOver()
        {
            StopAllCoroutines();
        }

        [Server]
        void PerformScan()
        {
            int size = Physics.OverlapBoxNonAlloc(_center, _halfBox, _results, _rotation, playerMask);
            
            if (size == 0)
            {
                if(ReferenceEquals(_king, null)) return;
                SetKing(null);
                return;
            }

            if (ReferenceEquals(_king, null))
            {
                SetKing(_results[0]);
                RewardPts();
                return;
            }
            
            bool constainsKing = false;
            for (int i = 0; i < size; i++)
            {
                if (ReferenceEquals(_results[i], _king))
                {
                    constainsKing = true;
                    break;
                }
            }
            
            if (!constainsKing)
            {
                SetKing(_results[0]);
            }
            
            RewardPts();
        }
        
        [Server]
        void SetKing(Collider king)
        {
            _king = king;

            if (ReferenceEquals(_king, null))
            {
                KingNulled();
                return;
            }
            
            _kingData = _king.GetComponentInParent<NetworkedSteamUser>().SteamUser;
            
            Debug.Log($"{_kingData.Nickname} is now King");
            
            KingChanged(_kingData);
        }

        [ObserversRpc]
        void KingChanged(UserData newKing)
        {
            kingChanged.Invoke(newKing);
            crownOnPedestal.SetActive(false);
        }

        [ObserversRpc]
        void KingNulled()
        {
            crownOnPedestal.SetActive(true);
            kingChanged.Invoke();
        }
        
        [ObserversRpc]
        void KingRewarded()
        {
            kingRewardedSound.Play();
        }

        [Server]
        void RewardPts()
        {
            requestChangeScore.Invoke(
                new NetworkedLeaderboard.ChangeScoreRequest(
                    _kingData, 
                    ptsPerSec, 
                    true));
            
            KingRewarded();
        }
    }
}
