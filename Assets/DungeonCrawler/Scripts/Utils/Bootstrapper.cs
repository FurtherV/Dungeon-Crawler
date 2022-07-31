using UnityEngine;

namespace DungeonCrawler.Scripts.Utils
{
    internal static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            var objects = Object.FindObjectsOfType<DungeonCrawler>();
            if (objects is { Length: > 0 })
                foreach (var parent in objects)
                    if (parent)
                        Object.Destroy(parent);
            Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));
            Debug.Log("Systems Singleton initialized.");
        }
    }
}