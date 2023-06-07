using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Wolfey
{
    public class ParticleSystemStopped : MonoBehaviour
    {
        [SerializeField] UnityEvent particleSystemStopped;
        
        void OnParticleSystemStopped()
        {
            particleSystemStopped.Invoke();
        }
    }
}
