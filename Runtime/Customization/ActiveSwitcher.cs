using UnityEngine;

namespace Wolfey
{
    public class ActiveSwitcher : MonoBehaviour
    {
        int _activeIdx;
        
        void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).gameObject.activeSelf) continue;
                _activeIdx = i;
                return;
            }
        }

        public void SetIdx(int idx)
        {
            _activeIdx = idx % transform.childCount;

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            transform.GetChild(_activeIdx).gameObject.SetActive(true);
        }

        public void Next()
        {
            SetIdx(_activeIdx + 1);
        }

        public void Previous()
        {
            SetIdx(_activeIdx - 1);
        }
    }
}
