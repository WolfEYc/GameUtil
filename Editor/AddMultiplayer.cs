#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AddMultiplayer
{
    [MenuItem("WolfeyGamedev/Add Multiplayer")]
    static void AddMultiplayerFunc()
    {
        Debug.Log("Multiplayer Added!");
    }
}
#endif
