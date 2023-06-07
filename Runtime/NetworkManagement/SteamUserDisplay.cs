using DG.Tweening;
using HeathenEngineering.SteamworksIntegration;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wolfey.Events;
using WolfeyFPS;

public class SteamUserDisplay : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] TMP_Text nickName;
    [SerializeField] AnimatedInt score;
    [SerializeField] EventObject leaderboardUpdated;
    [SerializeField] int rank = -1;
    [SerializeField] bool animateImmediately;
    [SerializeField] float shakeScale = 1f;
    [SerializeField] AudioClip impactClip;
    
    UserData _data;
    
    public bool Displaying { get; private set; }

    void Start()
    {
        LeaderboardUpdated();
    }

    public void LeaderboardUpdated()
    {
        NetworkedLeaderboard networkedLeaderboard = leaderboardUpdated.ReadValue<NetworkedLeaderboard>();
        if (rank == -1 || ReferenceEquals(networkedLeaderboard, null)
            || !networkedLeaderboard.TryGetUserAtRank(
                rank,
                out UserData user,
                out float denormalizedScore))
        {
            Hide();
            return;
        }
        
        Display(user, Mathf.CeilToInt(denormalizedScore));
    }

    public void Hide()
    {
        Displaying = false;
        image.enabled = false;
        nickName.SetText("");
        score.Hide();
    }
    
    public void Display(UserData data, int newScore = 0)
    {
        Displaying = true;
        _data = data;
        if (_data.Avatar != null)
        {
            AvatarLoaded();
        }
        else
        {
            image.enabled = false;
            image.texture = null;
            _data.LoadAvatar(AvatarLoaded);
        }
        
        nickName.SetText(_data.Nickname);
        score.TargetValue = newScore;
        
        if (animateImmediately)
        {
            AnimateScore(0f);
        }
    }

    public void AnimateScore(float delay = 0.1f)
    {
        if(!Displaying) return;
        UIAudio.PlayClip(impactClip);
        Invoke(nameof(PerformImpact), 0.1f);
        Invoke(nameof(PerformScoreAnimation), delay);
    }

    void PerformImpact()
    {
        transform.DOShakePosition(
            0.15f,
            new Vector3(30f, 30f, 0f) * shakeScale,
            90);
    }

    void PerformScoreAnimation()
    {
        score.Animate();
    }

    void AvatarLoaded(Texture2D avatar = null)
    {
        if(!Displaying) return;
        image.enabled = true;
        image.texture = _data.Avatar;
    }
}
