using UnityEngine;

namespace WolfeyFPS
{
    [CreateAssetMenu(menuName = "WolfeyFPS/Scene Info", fileName = "New Scene Info", order = 2)]
    public class SceneInfo : ScriptableObject
    {
        public Sprite sceneImage;
        public Sprite leaderboardImage;
    }
}
