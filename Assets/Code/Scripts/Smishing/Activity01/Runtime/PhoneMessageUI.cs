using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Smishing01
{
    /// <summary>
    /// World-space phone screen controller.
    /// </summary>
    public class PhoneMessageUI : MonoBehaviour
    {
        [Header("Text Fields")]
        public TMP_Text senderNameText;
        public TMP_Text messageBodyText;
        public TMP_Text embeddedUrlText;
        public TMP_Text timeStampText;
        public TMP_Text hintText;

        [Header("Action Buttons")]
        public Button reportButton;
        public Button ignoreButton;
        public Button clickLinkButton;
        public Button hintButton;

        [Header("Containers")]
        public CanvasGroup canvasGroup;
        public Transform  phoneBody;

        [Header("Confirm Dialog")]
        public ConfirmDialogController confirmDialog;

        public event Action<PlayerAction> OnPlayerAction;

        private SmishingScenarioData _current;
        private string _bodyOriginal;
        private string _urlOriginal;
        private bool   _hintUsed;

        public bool WasHintUsed => _hintUsed;

        // ── Public API ────────────────────────────────────────────────────────

        public void DisplayScenario(SmishingScenarioData data)
        {
            _current      = data;
            _bodyOriginal = data.messageBody;
            _urlOriginal  = data.embeddedUrl;
            _hintUsed     = false;

            if (senderNameText  != null) senderNameText.text  = data.senderName;
            if (messageBodyText != null) messageBodyText.text  = data.messageBody;
            if (timeStampText   != null) timeStampText.text    = System.DateTime.Now.ToString("HH:mm");

            if (embeddedUrlText != null)
            {
                embeddedUrlText.text = data.embeddedUrl;
                embeddedUrlText.gameObject.SetActive(!string.IsNullOrEmpty(data.embeddedUrl));
            }

            if (hintText != null)
            {
                hintText.text = "";
                hintText.gameObject.SetActive(false);
            }

            SetButtonsInteractable(true);
            if (hintButton != null) hintButton.interactable = true;

            gameObject.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(AnimateAppear());
        }

        /// <summary>Immediate hide — no animation race. Used between scenarios.</summary>
        public void Hide()
        {
            StopAllCoroutines();
            if (canvasGroup != null)
            {
                canvasGroup.alpha          = 0f;
                canvasGroup.interactable   = false;
                canvasGroup.blocksRaycasts = false;
            }
            gameObject.SetActive(false);
        }

        public void RevealRedFlags()
        {
            if (_current == null || string.IsNullOrEmpty(_current.suspiciousSubstring))
                return;

            string sub = _current.suspiciousSubstring;
            string highlighted = $"<color=#FF5050><u>{sub}</u></color>";

            if (messageBodyText != null && _bodyOriginal.Contains(sub))
                messageBodyText.text = _bodyOriginal.Replace(sub, highlighted);

            if (embeddedUrlText != null && !string.IsNullOrEmpty(_urlOriginal) && _urlOriginal.Contains(sub))
                embeddedUrlText.text = _urlOriginal.Replace(sub, highlighted);
        }

        // ── Unity lifecycle ───────────────────────────────────────────────────

        private void Awake()
        {
            // Wire buttons ONCE — these listeners persist across scenarios
            if (reportButton != null)
            {
                reportButton.onClick.RemoveAllListeners();
                reportButton.onClick.AddListener(() => Respond(PlayerAction.ReportMessage));
            }
            if (ignoreButton != null)
            {
                ignoreButton.onClick.RemoveAllListeners();
                ignoreButton.onClick.AddListener(() => Respond(PlayerAction.IgnoreMessage));
            }
            if (clickLinkButton != null)
            {
                clickLinkButton.onClick.RemoveAllListeners();
                clickLinkButton.onClick.AddListener(OnClickLinkPressed);
            }
            if (hintButton != null)
            {
                hintButton.onClick.RemoveAllListeners();
                hintButton.onClick.AddListener(OnHintPressed);
            }

            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

            // Start hidden, no animation flash
            if (canvasGroup != null)
            {
                canvasGroup.alpha          = 0f;
                canvasGroup.interactable   = false;
                canvasGroup.blocksRaycasts = false;
            }
            gameObject.SetActive(false);
        }

        // ── Private ───────────────────────────────────────────────────────────

        private void OnClickLinkPressed()
        {
            if (confirmDialog != null)
            {
                confirmDialog.Show(
                    title: "Are you sure?",
                    body : "Clicking unknown links can lead to credential theft or malware.\nProceed?",
                    onConfirm: () => Respond(PlayerAction.ClickLink),
                    onCancel : () => SetButtonsInteractable(true)
                );
                SetButtonsInteractable(false);
            }
            else
            {
                Respond(PlayerAction.ClickLink);
            }
        }

        private void OnHintPressed()
        {
            if (_hintUsed || _current == null || hintText == null) return;
            _hintUsed = true;
            if (hintButton != null) hintButton.interactable = false;
            hintText.text = "💡  " + _current.hintText;
            hintText.gameObject.SetActive(true);
            StartCoroutine(UITween.ScalePop(hintText.transform, 0.5f, 1f, 0.3f));
        }

        private void Respond(PlayerAction action)
        {
            SetButtonsInteractable(false);
            if (hintButton != null) hintButton.interactable = false;
            OnPlayerAction?.Invoke(action);
        }

        private void SetButtonsInteractable(bool value)
        {
            if (reportButton    != null) reportButton.interactable    = value;
            if (ignoreButton    != null) ignoreButton.interactable    = value;
            if (clickLinkButton != null) clickLinkButton.interactable = value;
        }

        private IEnumerator AnimateAppear()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha          = 0f;
                canvasGroup.interactable   = true;
                canvasGroup.blocksRaycasts = true;
            }
            Transform target = phoneBody != null ? phoneBody : transform;
            Coroutine fadeC = StartCoroutine(UITween.FadeCanvasGroup(canvasGroup, 0f, 1f, 0.35f));
            Coroutine popC  = StartCoroutine(UITween.ScalePop(target, 0.7f, 1f, 0.45f));
            yield return fadeC;
            yield return popC;
        }
    }
}
