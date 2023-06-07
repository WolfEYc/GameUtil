using UnityEngine;

namespace WolfeyFPS
{
    [CreateAssetMenu(menuName = "WolfeyFPS/Gamemode Info", fileName = "New Gamemode Info", order = 1)]
    public class Gamemode : ScriptableObject
    {
        public Sprite icon;
        public string[] descriptionRows;
    }
}
