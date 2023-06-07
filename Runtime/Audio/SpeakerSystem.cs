using System.Collections.Generic;
using UnityEngine;
using Wolfey.Systems;

namespace WolfeyFPS
{
    public class SpeakerSystem : Singleton<SpeakerSystem>
    {
        HashSet<Speaker> _speakers;

        protected override void Awake()
        {
            base.Awake();
            _speakers = new HashSet<Speaker>();
        }

        public void RegisterSpeaker(Speaker speaker)
        {
            _speakers.Add(speaker);
        }

        public void UnregisterSpeaker(Speaker speaker)
        {
            _speakers.Remove(speaker);
        }

        public static void Play(AudioClip clip)
        {
            foreach (var speaker in _instance._speakers)
            {
                speaker.AudioSource.clip = clip;
                speaker.AudioSource.Play();
            }
        }
    }
}
