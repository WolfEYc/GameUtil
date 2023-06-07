using UnityEngine;
using UnityEngine.Events;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class PlayAtTime : MonoBehaviour
    {
        [SerializeField] EventObject timeUpdated;
        [SerializeField] int playAtTime;
        [SerializeField] AudioClip clip;
        [SerializeField] UnityEvent onPlay;
        
        public void OnTimeUpdated()
        {
            int currentTime = timeUpdated.ReadValue<int>();
            
            if(currentTime != playAtTime) return;
            
            SpeakerSystem.Play(clip);
            onPlay.Invoke();
        }
    }
}
