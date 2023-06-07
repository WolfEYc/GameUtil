using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Wolfey.Events;

namespace WolfeyFPS
{
    public class GamemodeDisplay : MonoBehaviour
    {
        [SerializeField] EventObject gameInfoChanged;
        [SerializeField] TMP_Text gamemodeName;
        [SerializeField] TMP_Text sceneName;
        [SerializeField] TextColumn description;
        [SerializeField] Image sceneImage;
        [SerializeField] Image leaderboardImage;
        [SerializeField] Image gameModeIcon;
        
        public void OnGamemodeChange()
        {
            var gamemodeInfo = gameInfoChanged.ReadValue<Tuple<Gamemode, SceneInfo>>();
            
            gamemodeName.SetText(gamemodeInfo.Item1.name);
            sceneName.SetText(gamemodeInfo.Item2.name);
            description.AssignText(gamemodeInfo.Item1.descriptionRows);

            leaderboardImage.sprite = gamemodeInfo.Item2.leaderboardImage;
            sceneImage.sprite = gamemodeInfo.Item2.sceneImage;
            gameModeIcon.sprite = gamemodeInfo.Item1.icon;
        }
    }
}
