using FishNet.Connection;
using UnityEngine;


namespace WolfeyFPS
{
    public class ReceiveNotifData : NetworkEventData
    {
        public string Notif;
        public Color Color;
        
        public ReceiveNotifData(NetworkConnection sender, string notif) : base(sender)
        {
            Color = Color.white;
            Notif = notif;
        }
        
        public ReceiveNotifData(NetworkConnection sender, string notif, Color color) : base(sender)
        {
            Color = color;
            Notif = notif;
        }
    }
}
