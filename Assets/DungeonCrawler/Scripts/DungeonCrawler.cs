using DungeonCrawler.Scripts.Audio;
using DungeonCrawler.Scripts.Storage;
using DungeonCrawler.Scripts.Utils;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DungeonCrawler.Scripts
{
    public class DungeonCrawler : SingletonPersistent<DungeonCrawler>
    {
        #region Serialized Fields

        [SerializeField] private NetworkManager networkSystem;
        [SerializeField] private AudioSystem audioSystem;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private GameConfig activeConfiguration;

        #endregion

        [HideInInspector] public NetworkManager NetworkSystem => networkSystem;
        [HideInInspector] public AudioSystem AudioSystem => audioSystem;
        [HideInInspector] public EventSystem EventSystem => eventSystem;
        [HideInInspector] public GameConfig Configuration => activeConfiguration;

        /// <inheritdoc />
        protected override void AwakeLate()
        {
            activeConfiguration.ReadFromDisk();
        }
    }
}