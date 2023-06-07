using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Wolfey;

namespace WolfeyFPS
{
    [RequireComponent(typeof(TMP_Text))]
    public class AnimatedInt : MonoBehaviour
    {
        [SerializeField] float timeToReachTarget;
        TMP_Text _display;

        void Awake()
        {
            _display = GetComponent<TMP_Text>();
        }

        int _currentValue;
        public int TargetValue { get; set; }

        public void Animate()
        {
            StartCoroutine(CountText());
        }

        public void Hide()
        {
            _display.SetText("");
        }

        IEnumerator CountText()
        {
            int lastCurrentValue = _currentValue;
            float time = 0;

            do
            {
                yield return null;
                _currentValue = Mathf.CeilToInt(
                    Mathf.Lerp(
                        lastCurrentValue,
                        TargetValue,
                        time.Remap(0f, timeToReachTarget, 0f, 1f)));
                _display.SetText(_currentValue.ToString());
                time += Time.deltaTime;
            } while (_currentValue != TargetValue);
        }
    }
}
