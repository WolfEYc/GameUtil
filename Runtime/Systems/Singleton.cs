using UnityEngine;

namespace WolfeyGamedev.Systems
{
    public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;
        public static T Instance
        {
            get { return _instance ??= new GameObject(typeof(T).Name).AddComponent<T>(); }
        }

        protected virtual void Awake()
        {
            _instance = this as T;
        }

        protected virtual void OnApplicationQuit()
        {
            _instance = null;
            Destroy(gameObject);
        }
    }
    
    public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
        
            base.Awake();
        }
    }

    public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
    {
        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }
    }
}