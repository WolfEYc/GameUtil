using UnityEngine;

namespace WolfeyFPS
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerAudio : MonoBehaviour
    {
        AudioSource _source;

        void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void Play(AudioClip clip)
        {
            _source.Stop();
            _source.clip = clip;
            _source.Play();
        }
    }
}
