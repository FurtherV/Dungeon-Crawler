using UnityEngine;

namespace DungeonCrawler.Scripts.Utils
{
    public abstract class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        #region Event Functions

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
        }

        protected virtual void OnApplicationQuit()
        {
            Instance = null;
            Destroy(gameObject);
        }

        #endregion
    }

    public abstract class BehaviourSingletonPersistent<T> : BehaviourSingleton<T> where T : MonoBehaviour
    {
        #region Event Functions

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        #endregion
    }
}