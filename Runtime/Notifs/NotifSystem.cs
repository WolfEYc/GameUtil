using FishNet.Connection;
using FishNet.Object;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class NotifSystem : NetworkBehaviour
    {
        public EventObject sendNotifEvent;
        public EventObject receiveNotifEvent;

        public void OnNotifSend()
        {
            SendNotifData sendNotifData = sendNotifEvent.ReadValue<SendNotifData>();
            SendServer(sendNotifData.notif, sendNotifData.receivers);
        }

        [ServerRpc]
        void SendServer(string notif, NetworkConnection[] receivers)
        {
            if (receivers == null)
            {
                DisplayNotif(null, notif, Owner);
                return;
            }
            
            foreach (NetworkConnection receiver in receivers)
            {
                DisplayNotif(receiver, notif, Owner);
            }    
        }

        [TargetRpc]
        [ObserversRpc]
        void DisplayNotif(NetworkConnection target, string notif, NetworkConnection sender)
        {
            ReceiveNotifData data = new ReceiveNotifData(sender, notif);
            receiveNotifEvent.Invoke(data);
        }
    }
}
