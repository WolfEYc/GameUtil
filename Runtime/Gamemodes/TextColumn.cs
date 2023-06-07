using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WolfeyFPS
{
    public class TextColumn : MonoBehaviour
    {
        TMP_Text[] _texts;

        void Awake()
        {
            _texts = GetComponentsInChildren<TMP_Text>();
        }

        public void AssignText(string[] rows)
        {
            for (int i = 0; i < _texts.Length && i < rows.Length; i++)
            {
                _texts[i].SetText(rows[i]);
                LayoutRebuilder.ForceRebuildLayoutImmediate(_texts[i].rectTransform);
            }
        }
    }
}
