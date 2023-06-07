using UnityEngine;

namespace WolfeyFPS
{
    [CreateAssetMenu(menuName = "WolfeyFPS/Surface Sounds", fileName = "New Surface Sounds", order = 5)]
    public class SurfaceSounds : ScriptableObject
    {
        public AudioClip[] footStepSounds;
    }
}
