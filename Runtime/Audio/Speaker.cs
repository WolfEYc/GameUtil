using UnityEngine;

namespace WolfeyFPS
{
    [RequireComponent(typeof(AudioSource))]
    public class Speaker : MonoBehaviour
    {
        public AudioSource AudioSource { get; private set; }

        void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            SpeakerSystem.Instance.RegisterSpeaker(this);
        }

        void OnDisable()
        {
            if(SpeakerSystem.Instance == null) return;
            SpeakerSystem.Instance.UnregisterSpeaker(this);
        }
    }
}
