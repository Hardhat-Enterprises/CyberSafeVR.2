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
    /// The player presses a "Continue" button to advance.
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

        private bool _continueClicked;

        public Coroutine ShowOutcomeAndWait(bool correct, SmishingScenarioData data, Action onComplete)
        {
            return StartCoroutine(OutcomeRoutine(correct, data, onComplete));
        }

        public void LoadDebrief() => SceneManager.LoadScene(debriefSceneName);

        private void Awake()
        {
            HideAll();
            if (correctContinueButton != null)
            {
                correctContinueButton.onClick.RemoveAllListeners();
                correctContinueButton.onClick.AddListener(OnContinueClicked);
            }
            if (incorrectContinueButton != null)
            {
                incorrectContinueButton.onClick.RemoveAllListeners();
                incorrectContinueButton.onClick.AddListener(OnContinueClicked);
            }
        }

        private void OnContinueClicked() => _continueClicked = true;

        private IEnumerator OutcomeRoutine(bool correct, SmishingScenarioData data, Action onComplete)
        {
            _continueClicked = false;

            if (correct) ShowCorrect(data);
            else         ShowIncorrect(data);

            yield return new WaitUntil(() => _continueClicked);

            HideAll();
            yield return new WaitForSeconds(0.15f);
            onComplete?.Invoke();
        }

        private void ShowCorrect(SmishingScenarioData data)
        {
            HideAll();
            if (correctFeedbackText != null) correctFeedbackText.text = data.correctFeedback;
            if (correctPanel != null) correctPanel.SetActive(true);
            if (correctGroup != null)
            {
                correctGroup.alpha = 0f;
                StartCoroutine(UITween.FadeCanvasGroup(correctGroup, 0f, 1f, 0.3f));
            }
            if (correctPanel != null)
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

            if (incorrectPanel != null) incorrectPanel.SetActive(true);
            if (incorrectGroup != null)
            {
                incorrectGroup.alpha = 0f;
                StartCoroutine(UITween.FadeCanvasGroup(incorrectGroup, 0f, 1f, 0.3f));
            }
            if (incorrectPanel != null)
                StartCoroutine(UITween.ScalePop(incorrectPanel.transform, 0.85f, 1f, 0.35f));
        }

        private void HideAll()
        {
            if (correctPanel   != null) correctPanel.SetActive(false);
            if (incorrectPanel != null) incorrectPanel.SetActive(false);
        }
    }
}
