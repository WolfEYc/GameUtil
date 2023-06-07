using FishNet.Object;
using UnityEngine;
using Wolfey.Events;

public class NetworkTimer : NetworkBehaviour
{
    [SerializeField] float startTime;

    [SerializeField] EventObject timeStarted;
    [SerializeField] EventObject timeChanged;
    [SerializeField] EventObject timeEnded;
    [SerializeField] bool startOnAwake;
    
    int _prevRemainingTime;
    int _remaningTimeWholeNum;
    float _remainingTime;
    bool _timerActive;

    [ObserversRpc(BufferLast = true)]
    void OnTimeChange(int newWholeNum)
    {
        if (!_timerActive)
        {
            _timerActive = true;
            if(!ReferenceEquals(timeStarted, null))
                timeStarted.Invoke();
        }

        if (!ReferenceEquals(timeChanged, null))
        {
            timeChanged.Invoke(newWholeNum);
        }

        if (newWholeNum == 0 && !ReferenceEquals(timeEnded, null))
        {
            timeEnded.Invoke();
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (startOnAwake)
        {
            StartTimer();
        }
    }

    public void StartTimer()
    {
        if(!IsServer || _timerActive) return;

        _remainingTime = startTime;
        _remaningTimeWholeNum = (int)startTime;
        
        OnTimeChange(_remaningTimeWholeNum);
    }

    void FixedUpdate()
    {
        if(!_timerActive || !IsServer || _remaningTimeWholeNum == 0) return;
        _remainingTime = Mathf.Max(_remainingTime - Time.fixedDeltaTime, 0f);
        
        _remaningTimeWholeNum = (int)_remainingTime;
        if(_prevRemainingTime == _remaningTimeWholeNum) return;
        _prevRemainingTime = _remaningTimeWholeNum;
        OnTimeChange(_remaningTimeWholeNum);
    }
}
