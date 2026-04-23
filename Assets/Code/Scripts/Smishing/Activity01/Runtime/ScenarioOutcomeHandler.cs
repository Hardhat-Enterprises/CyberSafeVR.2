using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Smishing01
{
    /// <summary>
    /// Shows correct / incorrect feedback panels.
    /// The player presses a "Continue" button to advance instead of
    /// auto-closing, which gives them time to read the explanation.
    /// </summary>
    public class ScenarioOutcomeHandler : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject  correctPanel;
        public GameObject  incorrectPanel;
        public CanvasGroup correctGroup;
        public CanvasGroup incorrectGroup;

        [Header("Text")]
        public TMP_Text correctFeedbackText;
        public TMP_Text incorrectFeedbackText;
        public TMP_Text redFlagsText;

        [Header("Continue Buttons")]
        public Button correctContinueButton;
        public Button incorrectContinueButton;

        [Header("End Scene")]
        [SerializeField] private string debriefSceneName = "Smishing01_End";

        private Action _onContinue;

        // ── API ──────────────────────────────────────────────────────────────

        public Coroutine ShowOutcomeAndWait(bool correct, SmishingScenarioData data, Action onComplete)
        {
            return StartCoroutine(OutcomeRoutine(correct, data, onComplete));
        }

        public void LoadDebrief() => SceneManager.LoadScene(debriefSceneName);

        // ── Unity lifecycle ───────────────────────────────────────────────────

        private void Awake()
        {
            HideAll();
            correctContinueButton  ?.onClick.AddListener(() => { HideAll(); _onContinue?.Invoke(); });
            incorrectContinueButton?.onClick.AddListener(() => { HideAll(); _onContinue?.Invoke(); });
        }

        // ── Private ───────────────────────────────────────────────────────────

        private IEnumerator OutcomeRoutine(bool correct, SmishingScenarioData data, Action onComplete)
        {
            bool clicked = false;
            _onContinue = () => clicked = true;

            if (correct) ShowCorrect(data);
            else         ShowIncorrect(data);

            yield return new WaitUntil(() => clicked);
            onComplete?.Invoke();
        }

        private void ShowCorrect(SmishingScenarioData data)
        {
            HideAll();
            if (correctFeedbackText != null) correctFeedbackText.text = data.correctFeedback;
            correctPanel?.SetActive(true);
            if (correctGroup != null)
            {
                correctGroup.alpha = 0f;
                StartCoroutine(UITween.FadeCanvasGroup(correctGroup, 0f, 1f, 0.3f));
            }
            StartCoroutine(UITween.ScalePop(correctPanel.transform, 0.85f, 1f, 0.35f));
        }

        private void ShowIncorrect(SmishingScenarioData data)
        {
            HideAll();
            if (incorrectFeedbackText != null) incorrectFeedbackText.text = data.incorrectFeedback;
            if (redFlagsText != null && data.redFlags != null && data.redFlags.Length > 0)
                redFlagsText.text = "<b>Red flags</b>\n• " + string.Join("\n• ", data.redFlags);
            else if (redFlagsText != null)
                redFlagsText.text = "";

            incorrectPanel?.SetActive(true);
            if (incorrectGroup != null)
            {
                incorrectGroup.alpha = 0f;
                StartCoroutine(UITween.FadeCanvasGroup(incorrectGroup, 0f, 1f, 0.3f));
            }
            StartCoroutine(UITween.ScalePop(incorrectPanel.transform, 0.85f, 1f, 0.35f));
        }

        private void HideAll()
        {
            correctPanel  ?.SetActive(false);
            incorrectPanel?.SetActive(false);
        }
    }
}
