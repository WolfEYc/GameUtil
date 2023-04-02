using System;
using UnityEngine;
using UnityEngine.Events;

namespace WolfeyGamedev.Timer
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] float defaultStartTime;
        [SerializeField] bool startOnEnable;
        [SerializeField] UnityEvent onStarted, onExpired, onTimeAdded, onTimeDeducted;

        const float TimeChunk = 1f;
        
        public event Action Expired;
        public event Action Started;
        public event Action TimeAdded;
        public event Action TimeDeducted;
        
        public float RemainingTime { get; private set; }
        public bool Active { get; private set; }

        void OnEnable()
        {
            if(!startOnEnable) return;
            StartCountdown();
        }
        
        public void StartCountdown()
        {
            SetTime(defaultStartTime);
        }

        public void SetTime(float time)
        {
            RemainingTime = time;
            RegisterTimer();
            
            onStarted.Invoke();
            Started?.Invoke();
        }

        public void AddTime(float amt)
        {
            RemainingTime += amt;
            RegisterTimer();
            
            onTimeAdded.Invoke();
            TimeAdded?.Invoke();
        }

        public void DeductTime(float amt)
        {
            RemainingTime -= amt;
            
            if(amt < 1f) return;
            
            TimeDeducted?.Invoke();
            onTimeDeducted.Invoke();
        }

        public void StopCountdown()
        {
            if(!Active) return;
            TimerMaster.Instance.RemoveTimer(this);
            Active = false;
            RemainingTime = 0f;
        }

        public void Expire()
        {
            Active = false;
            RemainingTime = 0f;

            onExpired.Invoke();
            Expired?.Invoke();
        }

        void RegisterTimer()
        {
            if (Active) return;
            Active = true;
            TimerMaster.Instance.AddTimer(this);
        }
    }
}
