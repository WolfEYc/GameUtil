using TMPro;
using UnityEngine;

namespace WolfeyFPS
{
    [RequireComponent(typeof(TMP_Text))]
    public class ReadyDisplay : MonoBehaviour
    {
        TMP_Text _readyfraction;

        void Awake()
        {
            _readyfraction = GetComponent<TMP_Text>();
        }

        public void UpdateReadyness()
        {
            _readyfraction.SetText($"{SteamUsers.Instance.ReadyUsers()}/{SteamUsers.Instance.TotalUsers} Ready");
        }
    }
}
