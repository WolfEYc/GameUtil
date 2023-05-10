using UnityEngine;
using Wolfey.Extensions;

namespace Wolfey.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayRandomClip : MonoBehaviour
    {
        [SerializeField] AudioClip[] clips;
        AudioSource _source;

        void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void Play()
        {
            _source.clip = clips.RandomElement();
            _source.Play();
        }
    }
}
