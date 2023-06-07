using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using HeathenEngineering.SteamworksIntegration;
using UnityEngine;
using Wolfey.Events;

public class NetworkedLeaderboard : NetworkBehaviour
{
    public static NetworkedLeaderboard instance;
    
    [SerializeField] EventObject rankingsUpdated;
    [SerializeField] EventObject steamUserJoined;
    [SerializeField] EventObject steamUserLeft;
    [SerializeField] EventObject changeScoreEvent;
    
    public Dictionary<UserData, float> DenormalizedScores { get; private set; }
    UserData[] _rankings;
    
    void Awake()
    {
        instance = this;
        DenormalizedScores = new Dictionary<UserData, float>();
    }

    [ObserversRpc(BufferLast = true, ExcludeServer = true)]
    void UpdateObservers(UserData[] rankings, float[] denormalizedScores)
    {
        _rankings = rankings;

        DenormalizedScores.Clear();
        for (int i = 0; i < rankings.Length; i++)
        {
            DenormalizedScores[rankings[i]] = denormalizedScores[i];
        }
        
        rankingsUpdated.Invoke(this);
    }

    [Server]
    public void UpdateStanding(ChangeScoreRequest changeScoreRequest)
    {
        DenormalizedScores[changeScoreRequest.user] = changeScoreRequest.additive ? 
            changeScoreRequest.newDenormalizedRanking + DenormalizedScores[changeScoreRequest.user]
            : changeScoreRequest.newDenormalizedRanking;
        ServerUpdateStandings();
    }

    [Server]
    void ServerUpdateStandings()
    {
        _rankings = DenormalizedScores.Keys.OrderByDescending(data => DenormalizedScores[data]).ToArray();
        
        float[] scores = new float[_rankings.Length];
        for (int i = 0; i < scores.Length; i++)
        {
            scores[i] = DenormalizedScores[_rankings[i]];
        }
        
        UpdateObservers(_rankings, scores);
        rankingsUpdated.Invoke(this);
    }

    public bool TryGetUserAtRank(int normalizedRank, out UserData user, out float denormalizedScore)
    {
        user = default;
        denormalizedScore = 0f;
        if (normalizedRank < 0 || normalizedRank >= _rankings.Length) return false;
        user = _rankings[normalizedRank];
        denormalizedScore = DenormalizedScores[user];
        return true;
    }
    
    public void OnSteamUserLeft()
    {
        if(!IsServer)return;
        UserData steamUser = steamUserLeft.ReadValue<UserData>();
        
        DenormalizedScores.Remove(steamUser);
        ServerUpdateStandings();
    }
    
    public void OnSteamUserJoined()
    {
        if(!IsServer) return;
        UserData steamUser = steamUserJoined.ReadValue<UserData>();
        UpdateStanding(new ChangeScoreRequest(steamUser, 0f));
    }

    [Server]
    public void ProcessChangeScoreRequest()
    {
        ChangeScoreRequest changeScoreRequest = changeScoreEvent.ReadValue<ChangeScoreRequest>();
        UpdateStanding(changeScoreRequest);
    }

    public void OnNewGameStarted()
    {
        if(!IsServer) return;
        
        for (int i = 0; i < _rankings.Length; i++)
        {
            DenormalizedScores[_rankings[i]] = 0f;
        }
        ServerUpdateStandings();
    }

    public UserData CurrentKing()
    {
        return _rankings[0];
    }

    public NetworkObject CurrentKingNob()
    {
        return NetworkedSteamUser.Usermap[CurrentKing()];
    }
    
    public struct ChangeScoreRequest
    {
        public UserData user;
        public float newDenormalizedRanking;
        public bool additive;

        public ChangeScoreRequest(UserData userdata, float newDenormalizedRanking, bool additive = false)
        {
            user = userdata;
            this.newDenormalizedRanking = newDenormalizedRanking;
            this.additive = additive;
        }
    }
}
