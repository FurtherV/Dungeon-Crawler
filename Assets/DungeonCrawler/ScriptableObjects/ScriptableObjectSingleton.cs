using System;
using UnityEngine;

namespace DungeonCrawler.ScriptableObjects
{
    public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var results = Resources.FindObjectsOfTypeAll<T>();
                switch (results.Length)
                {
                    case 0:
                        throw new Exception($"Could not find scriptable object instance of type {typeof(T)}.");
                    case > 1:
                        throw new Exception(
                            $"Found multiple scriptable object instances of type {typeof(T)}, only one is allowed.");
                }

                _instance = results[0];
                _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;

                return _instance;
            }
        }
    }
}