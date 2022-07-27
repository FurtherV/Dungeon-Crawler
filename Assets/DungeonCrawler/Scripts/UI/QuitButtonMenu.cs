using UnityEditor;
using UnityEngine;

namespace DungeonCrawler.Scripts.UI
{
    public class QuitButtonMenu : MonoBehaviour
    {
        public void DoQuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit(0);
#endif
        }
    }
}