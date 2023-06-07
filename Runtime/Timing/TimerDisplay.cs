using System;
using TMPro;
using UnityEngine;
using Wolfey.Events;

[RequireComponent(typeof(TMP_Text))]
public class TimerDisplay : MonoBehaviour
{
    [SerializeField] EventObject timeChanged;
    TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    public void OnTimeChanged()
    {
        TimeSpan time = TimeSpan.FromSeconds(timeChanged.ReadValue<int>());
        _text.SetText(time.ToString(@"mm\:ss"));
    }
}
