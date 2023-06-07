using UnityEngine;
using Wolfey;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class KillFeed : MonoBehaviour
    {
        [SerializeField] KillFeedSingleDisplay singleDisplayPrefab;
        [SerializeField] EventObject deathEvent;
        
        public void DisplayKill()
        {
            DeathData deathData = deathEvent.ReadValue<DeathData>();
            
            Instantiate(singleDisplayPrefab, transform).Display(deathData);
        }

        public void WipeKillFeed()
        {
            transform.DestroyChildren();
        }
    }
}
