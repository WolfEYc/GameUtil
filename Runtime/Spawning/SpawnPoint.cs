using UnityEngine;

namespace WolfeyFPS
{
    public class SpawnPoint : MonoBehaviour
    {
        public Vector2 Rot => transform.rotation.eulerAngles;
        public Vector3 Pos => transform.position;
    }
}
