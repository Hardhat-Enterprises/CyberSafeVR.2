using UnityEngine;
using UnityEngine.UI;

namespace InsiderThreat01 {
    public class MCQHandler : MonoBehaviour
    {
        [Header("Answer Buttons")]
        public Button[] answerButtons;

        [Header("Index of Correct Answer (0-based)")]
        public int correctIndex;

        [Header("Feedback Panel for this Question")]
        public GameObject feedbackPanel;

        private bool answeredCorrectly = false;

        public Color normalColor = new Color(1f, 0.7f, 0.7f, 1f);
        public Color wrongColor = new Color(0.9f, 0.2f, 0.2f, 1f);
        public Color rightColor = new Color(0.2f, 0.8f, 0.2f, 1f);

        void Start()
        {
            for (int i = 0; i < answerButtons.Length; i++)
            {
                int idx = i;
                answerButtons[i].onClick.AddListener(() => OnAnswerSelected(idx));
                SetButtonColor(answerButtons[i], normalColor);
            }

            if (feedbackPanel != null) feedbackPanel.SetActive(false);
        }

        void OnEnable()
        {
            answeredCorrectly = false;
            foreach (var b in answerButtons)
            {
                b.interactable = true;
                SetButtonColor(b, normalColor);
            }
        }

        void OnAnswerSelected(int index)
        {
            if (answeredCorrectly) return;

            if (index == correctIndex)
            {
                // Mark correct button green
                SetButtonColor(answerButtons[index], rightColor);
                answeredCorrectly = true;

                // Disable quiz
                gameObject.SetActive(false);

                // Show feedback
                if (feedbackPanel != null) feedbackPanel.SetActive(true);
            }
            else
            {
                // Wrong → button turns red, but quiz stays active
                SetButtonColor(answerButtons[index], wrongColor);
                answerButtons[index].interactable = false;
            }
        }

        void SetButtonColor(Button btn, Color c)
        {
            var img = btn.GetComponent<Image>();
            if (img) img.color = c;
        }
    }
}


