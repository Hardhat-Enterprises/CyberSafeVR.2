using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Smishing01
{
    /// <summary>
    /// HUD Canvas — top bar that always shows:
    ///  • Scenario title
    ///  • Narration / guidance text
    ///  • A row of dots showing completed / remaining scenarios
    ///  • A running score count
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Text")]
        public TMP_Text titleText;
        public TMP_Text narrationText;
        public TMP_Text scoreText;
        public TMP_Text stepLabel;

        [Header("Progress dots")]
        public Transform dotsContainer;
        public Image     dotTemplate;

        [Header("Dot colours")]
        public Color dotPending = new Color(0.25f, 0.25f, 0.28f, 1f);
        public Color dotCurrent = new Color(1f,    0.82f, 0.25f, 1f);
        public Color dotCorrect = new Color(0.25f, 0.80f, 0.40f, 1f);
        public Color dotWrong   = new Color(0.90f, 0.28f, 0.28f, 1f);

        private readonly List<Image> _dots = new List<Image>();

        public void BuildDots(int count)
        {
            // Clear children except template
            foreach (Transform c in dotsContainer)
                if (c != dotTemplate?.transform) Destroy(c.gameObject);
            _dots.Clear();

            if (dotTemplate == null || dotsContainer == null) return;

            dotTemplate.gameObject.SetActive(false);

            for (int i = 0; i < count; i++)
            {
                var dot = Instantiate(dotTemplate, dotsContainer);
                dot.gameObject.SetActive(true);
                dot.color = dotPending;
                _dots.Add(dot);
            }
        }

        public void SetCurrentStep(int stepIndex)
        {
            for (int i = 0; i < _dots.Count; i++)
                if (_dots[i].color == dotPending && i == stepIndex)
                    _dots[i].color = dotCurrent;

            if (stepLabel != null)
                stepLabel.text = $"Step {stepIndex + 1} of {_dots.Count}";
        }

        public void MarkStepResult(int stepIndex, bool correct)
        {
            if (stepIndex < 0 || stepIndex >= _dots.Count) return;
            _dots[stepIndex].color = correct ? dotCorrect : dotWrong;
            StartCoroutine(UITween.ScalePop(_dots[stepIndex].transform, 0.5f, 1f, 0.35f));
        }

        public void ShowNarration(string title, string narration)
        {
            if (titleText     != null) titleText.text     = title;
            if (narrationText != null) narrationText.text = narration;
        }

        public void SetScore(int correct, int total)
        {
            if (scoreText != null) scoreText.text = $"Score: {correct} / {total}";
        }
    }
}
