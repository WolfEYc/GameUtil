#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using Wolfey;

namespace WolfeyGamedev
{
    public class PlaceInCircle : EditorWindow
    {
        [MenuItem("WolfeyGamedev/PlaceInCircle")]
        static void Init()
        {
            PlaceInCircle window = (PlaceInCircle)GetWindow(typeof(PlaceInCircle));
            window.Show();
        }

        float _radius = 0f;

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Radius", EditorStyles.boldLabel);
            _radius = EditorGUILayout.FloatField("", _radius);

            if (GUILayout.Button("PlaceInCircle"))
            {
                int idx = 0;
                foreach (GameObject go in Selection.gameObjects)
                {
                    Undo.RecordObject(go.transform, "PlaceInCircle");

                    go.transform.PlaceInCircle(_radius, idx, Selection.gameObjects.Length);
                    idx++;
                }
            }

            if (GUILayout.Button("PlaceInSemiCircle"))
            {
                int idx = 0;
                foreach (GameObject go in Selection.gameObjects)
                {
                    Undo.RecordObject(go.transform, "PlaceInCircle");

                    go.transform.PlaceInSemiCircle(_radius, idx, Selection.gameObjects.Length);
                    idx++;
                }
            }
        }
    }

}

#endif
