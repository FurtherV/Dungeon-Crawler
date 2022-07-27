using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DungeonCrawler.Scripts.UI
{
    public class DialogController : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI Components")] public Button confirmButton;

        public Button cancelButton;
        public TMP_InputField inputField;

        [Header("Settings")]
        public bool autoClose = true;
        public bool canBeCancelled = true;

        [Header("Events")] public UnityEvent onDialogOpen;

        public UnityEvent onDialogClose;
        public UnityEvent<string> onConfirm;
        public UnityEvent onCancel;

        #endregion

        public string Result => inputField ? inputField.text : "";
        
        #region Event Functions

#if UNITY_EDITOR
        private void Reset()
        {
            var childrenButton = GetComponentsInChildren<Button>();
            if ((childrenButton != null) && (childrenButton.Length > 0))
                foreach (var button in childrenButton)
                {
                    if (button.gameObject.name.Contains("confirm", StringComparison.InvariantCultureIgnoreCase))
                        confirmButton = button;
                    if (button.gameObject.name.Contains("cancel", StringComparison.InvariantCultureIgnoreCase))
                        cancelButton = button;
                }

            inputField = GetComponentInChildren<TMP_InputField>();
        }
#endif

        private void Awake()
        {
            if (!confirmButton) throw new NullReferenceException("No confirm button is set.");
            confirmButton.onClick.AddListener(Confirm);
            
            if (cancelButton)
            {
                cancelButton.onClick.AddListener(Cancel);
                if (!canBeCancelled)
                {
                    cancelButton.gameObject.SetActive(false);
                }
            }
        }

        #endregion

        public virtual void Confirm()
        {
            onConfirm?.Invoke(Result);
            if (autoClose) CloseDialog();
        }

        public virtual void Cancel()
        {
            onCancel?.Invoke();
            if (autoClose) CloseDialog();
        }

        public virtual void OpenDialog()
        {
            Debug.Log("OpenDialog called");
            Debug.Log($"Active states Pre: {gameObject.activeSelf}/{gameObject.activeInHierarchy}");
            gameObject.SetActive(true);
            Debug.Log($"Active states Post: {gameObject.activeSelf}/{gameObject.activeInHierarchy}");
            onDialogOpen?.Invoke();
        }

        public virtual void CloseDialog()
        {
            gameObject.SetActive(false);
            onDialogClose?.Invoke();
        }
    }
}