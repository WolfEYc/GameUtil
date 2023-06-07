using UnityEngine;
using Wolfey;

namespace WolfeyFPS
{
    public class Footstepper : MonoBehaviour
    {
        [SerializeField] GroundCheck gCheck;
        [SerializeField] AudioSource footStepSource;
        [SerializeField] float cd;

        float _lastPlayedAt;

        bool OnCd => _lastPlayedAt + cd > Time.time;
        
        public void HandleFootReachLowest()
        {
            if(!gCheck.Grounded || OnCd) return;
            _lastPlayedAt = Time.time;
            footStepSource.PlayOneShot(gCheck.SurfaceSounds.footStepSounds.RandomElement(), 0.4f);
        }
    }
}
