using System.Collections.Generic;
using UnityEngine;
using WolfeyGamedev.Systems;


namespace WolfeyGamedev.Timer
{
    public class TimerMaster : PersistentSingleton<TimerMaster>
    {
        List<Timer> _timers;
        List<Timer> _swap;

        public TimerMaster(){
            _timers = new List<Timer>();
            _swap = new List<Timer>();
        }

        /// <summary>
        /// Adds a Timer to be used ECS Style
        /// </summary>
        /// <param name="timer">Timer to be added</param>
        public void AddTimer(Timer timer)
        {
            _timers.Add(timer);
            
            if(_swap.Capacity == _timers.Capacity) return;
            _swap.Capacity = _timers.Capacity;
        }

        public void RemoveTimer(Timer timer)
        {
            _timers.Remove(timer);
        }
        
        public void FixedUpdate()
        {
            float timeDelta = Time.fixedDeltaTime;
            _swap.Clear();
            
            for(int i = 0; i < _timers.Count; i++)
            {
                _timers[i].DeductTime(timeDelta);
                
                if (_timers[i].RemainingTime > 0f && _timers[i].Active)
                {
                    _swap.Add(_timers[i]);
                    continue;
                }
                
                _timers[i].Expire();
            }
            
            if(_swap.Count == _timers.Count) return;
            
            _timers.Clear();
            _timers.AddRange(_swap);
        }
    }
}
