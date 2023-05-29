using UnityEngine;
using Wolfey.Extensions;

namespace Wolfey.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayRandomClip : MonoBehaviour
    {
        [SerializeField] AudioClip[] clips;
        [SerializeField] bool playOnAwake;
        AudioSource _source;

        void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            if (playOnAwake)
            {
                Play();
            }
        }

        public void Play()
        {
            _source.PlayOneShot(clips.RandomElement());
        }
    }
}
