using System;
using DungeonCrawler.Scripts.Storage;
using UnityEngine;

namespace DungeonCrawler.Scripts.Audio
{
    public class AudioSystem : MonoBehaviour
    {
        public AudioSource musicSource;
        public AudioSource sfxSource;

        private void Start()
        {
            UpdateMusicVolume();
            UpdateSFXVolume();
            DungeonCrawler.Instance.Configuration.onPropertyChanged.AddListener(OnConfigPropertyChanged);
        }

        private void UpdateMusicVolume()
        {
            musicSource.volume = Mathf.Clamp01(DungeonCrawler.Instance.Configuration.MusicVolume / 100f);
        }

        private void UpdateSFXVolume()
        {
            sfxSource.volume = Mathf.Clamp01(DungeonCrawler.Instance.Configuration.SfxVolume / 100f);
        }

        private void OnConfigPropertyChanged(string propertyName)
        {
            var updateAlways = string.IsNullOrEmpty(propertyName);
            if (propertyName == nameof(GameConfig.MusicVolume) || updateAlways)
            {
                UpdateMusicVolume();
            }
            if (propertyName == nameof(GameConfig.SfxVolume) || updateAlways)
            {
                UpdateSFXVolume();
            }
        }
    }
}