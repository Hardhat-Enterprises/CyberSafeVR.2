using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Smishing01
{
    /// <summary>
    /// The welcome / briefing screen shown before the first scenario.
    /// Gives the player context about what they're about to do and a
    /// clear "Begin" button to start the activity.
    /// </summary>
    public class IntroScreenController : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public TMP_Text    titleText;
        public TMP_Text    briefingText;
        public Button      startButton;

        public event Action OnStartRequested;

        private void Awake()
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            startButton?.onClick.AddListener(() =>
            {
                OnStartRequested?.Invoke();
                Hide();
            });
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                StopAllCoroutines();
                StartCoroutine(UITween.FadeCanvasGroup(canvasGroup, 0f, 1f, 0.4f));
                canvasGroup.interactable   = true;
                canvasGroup.blocksRaycasts = true;
            }
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
