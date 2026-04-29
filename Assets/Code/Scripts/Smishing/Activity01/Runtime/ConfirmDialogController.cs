using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Smishing01
{
    /// <summary>
    /// Modal confirmation dialog. Used when the player is about to take a
    /// risky action ("Click Link") — emphasises that clicking unknown links
    /// is itself a meaningful decision.
    /// </summary>
    public class ConfirmDialogController : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public TMP_Text    titleText;
        public TMP_Text    bodyText;
        public Button      confirmButton;
        public Button      cancelButton;

        private Action _onConfirm;
        private Action _onCancel;

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            confirmButton?.onClick.AddListener(() => { Hide(); _onConfirm?.Invoke(); });
            cancelButton?.onClick.AddListener (() => { Hide(); _onCancel?.Invoke();  });
            Hide();
        }

        public void Show(string title, string body, Action onConfirm, Action onCancel)
        {
            if (titleText != null) titleText.text = title;
            if (bodyText  != null) bodyText.text  = body;
            _onConfirm = onConfirm;
            _onCancel  = onCancel;

            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                StopAllCoroutines();
                StartCoroutine(UITween.FadeCanvasGroup(canvasGroup, 0f, 1f, 0.25f));
                canvasGroup.interactable   = true;
                canvasGroup.blocksRaycasts = true;
            }
            StartCoroutine(UITween.ScalePop(transform, 0.8f, 1f, 0.25f));
        }

        public void Hide()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha          = 0f;
                canvasGroup.interactable   = false;
                canvasGroup.blocksRaycasts = false;
            }
            gameObject.SetActive(false);
        }
    }
}
