using System;
using UnityEngine;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class GamemodeInfo : MonoBehaviour
    {
        [SerializeField] EventObject gameInfoChanged;
        [SerializeField] Gamemode gamemode;
        [SerializeField] SceneInfo sceneInfo;
        
        void Awake()
        {
            gameInfoChanged.Invoke(new Tuple<Gamemode, SceneInfo>(gamemode, sceneInfo));
        }
    }
}
