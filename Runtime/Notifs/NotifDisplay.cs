using TMPro;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class NotifDisplay : MonoBehaviour
    {
        [SerializeField] EventObject receiveNotifEvent;
        [SerializeField] TMP_Text notifText;
        [SerializeField] Animator animator;
        
        public static readonly int Notif = Animator.StringToHash("Notif");

        public void DisplayNotif()
        {
            ReceiveNotifData notifData = receiveNotifEvent.ReadValue<ReceiveNotifData>();
            
            notifText.SetText(notifData.Notif);
            notifText.color = notifData.Color;
            
            animator.SetTrigger(Notif);
        }
    }
}
