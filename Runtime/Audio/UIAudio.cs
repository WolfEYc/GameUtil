using UnityEngine;
using Wolfey.Systems;

namespace WolfeyFPS
{
    [RequireComponent(typeof(AudioSource))]
    public class UIAudio : Singleton<UIAudio>
    {
        public AudioSource uiAudioSource { get; private set; }
        [SerializeField] AudioClip inputFailedSound;

        protected override void Awake()
        {
            base.Awake();
            uiAudioSource = GetComponent<AudioSource>();
        }

        public static void PlayClip(AudioClip clip)
        {
            _instance.uiAudioSource.clip = clip;
            _instance.uiAudioSource.Play();
        }

        public static void PlayInputFailedAudio()
        {
            PlayClip(_instance.inputFailedSound);
        }
    }
}
