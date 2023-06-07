#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class ClearColliders
{
    [MenuItem("WolfeyGamedev/Destroy Colliders")]
    static void ClearCollidersOnGO()
    {
        foreach (GameObject go in Selection.gameObjects)
        {
            foreach (var collider in go.GetComponentsInChildren<Collider>())
            {
                Object.DestroyImmediate(collider);
            }
        }
    }
}

#endif
