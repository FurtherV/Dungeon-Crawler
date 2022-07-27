using System.IO;
using System.Linq;
using UnityEngine;

namespace DungeonCrawler.ScriptableObjects
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = nameof(DungeonCrawler) + "/GameConfig", order = 0)]
    public class GameConfig : ScriptableObjectSingleton<GameConfig>
    {
        #region Serialized Fields

        [field: SerializeField] public string PlayerName { set; get; }
        [SerializeField] private int musicVolume;
        [SerializeField] private int effectVolume;

        #endregion

        public void WriteToDisk(bool overwrite = true)
        {
            if (!overwrite && ExistsOnDisk()) return;
            var json = JsonUtility.ToJson(this, true);
            File.WriteAllText(GetFilePath(), json);
        }

        public void ReadFromDisk()
        {
            if (!ExistsOnDisk()) return;
            var json = File.ReadAllText(GetFilePath());
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public bool ExistsOnDisk()
        {
            return File.Exists(GetFilePath());
        }

        public string GetFilePath()
        {
            return Path.Combine(Application.persistentDataPath, "gameconfig.json");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void RuntimeInitializeOnLoad()
        {
            Instance.ReadFromDisk();
        }
    }
}