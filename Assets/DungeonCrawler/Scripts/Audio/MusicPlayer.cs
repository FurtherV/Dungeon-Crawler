using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonCrawler.Scripts.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private List<AudioClip> audioClips;
        [SerializeField] private bool shuffle = true;
        [SerializeField] private AudioSource audioSource;

        #endregion

        #region Event Functions

        private void Start()
        {
            audioSource.playOnAwake = true;
            switch (audioClips.Count)
            {
                case 0:
                    return;
                case 1:
                    audioSource.clip = audioClips.FirstOrDefault();
                    audioSource.loop = true;
                    audioSource.Play();
                    return;
                default:
                    if (shuffle) Shuffle();
                    StartCoroutine(Play());
                    break;
            }
        }

        #endregion

        private IEnumerator Play()
        {
            var nextClip = 0;
            while (nextClip < audioClips.Count)
            {
                audioSource.clip = audioClips[nextClip];
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length);
                if (nextClip == audioClips.Count - 1)
                    nextClip = 0;
                else
                    nextClip++;
            }

            yield return null;
        }

        public void Shuffle()
        {
            var n = audioClips.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Range(0, n + 1);
                (audioClips[k], audioClips[n]) = (audioClips[n], audioClips[k]);
            }
        }
    }
}