using FishNet.Connection;

namespace WolfeyFPS
{
    public class SendNotifData
    {
        public NetworkConnection[] receivers;
        public string notif;

        /// <summary>
        /// Data required to send a notification
        /// </summary>
        /// <param name="owner">Sender</param>
        /// <param name="notif">Notification</param>
        /// <param name="receivers">Receivers of the notif, leave null to send to all</param>
        public SendNotifData(string notif, NetworkConnection[] receivers = null)
        {
            this.notif = notif;
            this.receivers = receivers;
        }
    }
}
