using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WolfeyFPS
{
    public class KillFeedSingleDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text killerName;
        [SerializeField] Image killTool;
        [SerializeField] TMP_Text deadGuyName;
        [SerializeField] Image bg;
        [SerializeField] Image border;
        [SerializeField] Color selfDeathColor;
        [SerializeField] Color killOtherBorderColor;
        [SerializeField] Color killOtherBgColor;
        [SerializeField] float ttl;
        
        public void Display(DeathData deathData)
        {
            Invoke(nameof(Vaporize), ttl);
            
            deadGuyName.SetText(
                deathData.DeadObject.GetComponent<NetworkedSteamUser>().SteamUser.Nickname);

            bool selfDeath = deathData.DeadObject.Owner.IsLocalClient;
            
            if (selfDeath)
            {
                bg.color = selfDeathColor;
            }
            
            if(ReferenceEquals(deathData.Killer, null) || !deathData.Killer.Owner.IsLocalClient || selfDeath)
            {
                return;
            }
            
            bg.color = killOtherBgColor;
            border.color = killOtherBorderColor;
            
            killerName.SetText(
                deathData.Killer.GetComponent<NetworkedSteamUser>().SteamUser.Nickname);
            killTool.sprite = deathData.killTool;
        }

        public void Vaporize()
        {
            Destroy(gameObject);
        }
    }
}
