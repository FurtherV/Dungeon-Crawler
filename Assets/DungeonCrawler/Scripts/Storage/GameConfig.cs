using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

namespace DungeonCrawler.Scripts.Storage
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "DungeonCrawler/GameConfig", order = 0),
     JsonObject(MemberSerialization.OptIn)]
    public class GameConfig : ScriptableObject
    {
        private string ConfigFilePath => Path.Combine(Application.persistentDataPath, "gameconfig.json");

        #region Serialized Fields
        
        [Header("Fields")]
        [SerializeField] private string playerName = null;
        [SerializeField] private float musicVolume = 50f;
        [SerializeField] private float sfxVolume = 50f;
        
        [Header("Events")]
        public UnityEvent<string> onPropertyChanged;

        #endregion

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate | DefaultValueHandling.Include), DefaultValue(null)]
        public string PlayerName
        {
            get => playerName;
            set
            {
                playerName = value;
                RaiseOnPropertyChanged();
            }
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate | DefaultValueHandling.Include), DefaultValue(50f)]
        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = value;
                RaiseOnPropertyChanged();
            }
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate | DefaultValueHandling.Include), DefaultValue(50f)]
        public float SfxVolume
        {
            get => sfxVolume;
            set
            {
                sfxVolume = value;
                RaiseOnPropertyChanged();
            }
        }

        public void WriteToDisk()
        {
            var path = ConfigFilePath;
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            Directory.GetParent(path)?.Create();
            File.WriteAllText(path, json);
        }

        public void ReadFromDisk()
        {
            var path = ConfigFilePath;
            if (!File.Exists(path))
            {
                Debug.Log("Configuration file was missing, creating default...");
                var so = CreateInstance<GameConfig>();
                so.WriteToDisk();
            }

            var json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("Configuration file was empty, regenerating default...");
                WriteToDisk();
                json = File.ReadAllText(path);
            }

            JsonConvert.PopulateObject(json, this);
        }

        private void RaiseOnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            onPropertyChanged?.Invoke(propertyName);
        }
    }
}