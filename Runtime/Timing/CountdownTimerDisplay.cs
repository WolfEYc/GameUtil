using TMPro;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class CountdownTimerDisplay : MonoBehaviour
    {
        [SerializeField] EventObject startTimeUpdated;
        [SerializeField] int maxDisplaySeconds;
        [SerializeField] AudioClip[] playAtSecond;
        [SerializeField] Animator textAnimator;
        [SerializeField] TMP_Text text;
        
        public void DisplayTime()
        {
            int currentTime = startTimeUpdated.ReadValue<int>();
            
            if(currentTime >= maxDisplaySeconds) return;
            
            SpeakerSystem.Play(playAtSecond[currentTime]);
            text.SetText(currentTime.ToString());
            textAnimator.SetTrigger(NotifDisplay.Notif);
        }
    }
}
