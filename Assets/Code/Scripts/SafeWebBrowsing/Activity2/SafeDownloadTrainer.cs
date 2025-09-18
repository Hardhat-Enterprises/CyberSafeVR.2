using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SWB02
{
    public class SafeDownloadTrainer : MonoBehaviour
    {
        [System.Serializable]
        public class Question
        {
            [TextArea(1, 4)] public string prompt;
            public string[] options = new string[3];
            public int correctIndex; // 0..2
        }

        [Header("Rig & Movement")]
        public Transform xrOrigin;           // XR Origin (XR Rig)
        public Transform quizSpot;           // Target transform to glide to
        public float moveDuration = 2f;      // seconds
        public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Panels")]
        public GameObject introPanel;
        public GameObject readyPanel;
        public GameObject quizPanel;
        public GameObject endPanel;

        [Header("Intro UI")]
        public TMP_Text introTitle;
        public TMP_Text introBody;
        public Button startButton;

        [Header("Ready UI")]
        public TMP_Text readyPrompt;
        public Button readyButton;

        [Header("Quiz UI")]
        public TMP_Text questionText;
        public Button[] optionButtons;        // size 3
        public TMP_Text[] optionButtonLabels; // size 3 (child TMPs)
        public TMP_Text feedbackText;

        [Header("End UI")]
        public TMP_Text endTitle;
        public TMP_Text endBody;

        [Header("Audio (optional)")]
        public AudioSource audioSource;       // can be left empty
        public AudioClip correctClip;         // can be left empty
        public AudioClip wrongClip;           // can be left empty

        [Header("Questions")]
        public Question[] questions;

        private int currentQuestion = -1;
        private bool waitingForAnswer = false;

        void Awake()
        {
            ShowOnly(introPanel);

            if (introTitle) introTitle.text = "Safe Download Decision-Making";
            if (introBody) introBody.text =
                "Tips:\n• Prefer official HTTPS sources\n• Check extensions (.pdf/.docx ok; be wary of .exe/.bat)\n• Watch unusual sizes & urgency pop-ups";
            if (readyPrompt) readyPrompt.text = "Are you ready?";
            if (endTitle) endTitle.text = "Nice job!";
            if (endBody) endBody.text = "Thanks for completing the activity. Stay safe online!";

            if (startButton) startButton.onClick.AddListener(OnStart);
            if (readyButton) readyButton.onClick.AddListener(OnReady);

            // Wire option buttons
            if (optionButtons != null)
            {
                for (int i = 0; i < optionButtons.Length; i++)
                {
                    int ix = i;
                    if (optionButtons[i] != null)
                        optionButtons[i].onClick.AddListener(() => OnOption(ix));
                }
            }
        }

        void ShowOnly(GameObject panelToShow)
        {
            if (introPanel) introPanel.SetActive(panelToShow == introPanel);
            if (readyPanel) readyPanel.SetActive(panelToShow == readyPanel);
            if (quizPanel) quizPanel.SetActive(panelToShow == quizPanel);
            if (endPanel) endPanel.SetActive(panelToShow == endPanel);
        }

        void OnStart()
        {
            StartCoroutine(MoveRigThenShowReady());
        }

        IEnumerator MoveRigThenShowReady()
        {
            if (xrOrigin && quizSpot)
            {
                Vector3 p0 = xrOrigin.position, p1 = quizSpot.position;
                Quaternion r0 = xrOrigin.rotation, r1 = quizSpot.rotation;
                float t = 0f;

                while (t < 1f)
                {
                    t += Time.deltaTime / Mathf.Max(0.01f, moveDuration);
                    float k = moveCurve.Evaluate(Mathf.Clamp01(t));
                    xrOrigin.position = Vector3.Lerp(p0, p1, k);
                    xrOrigin.rotation = Quaternion.Slerp(r0, r1, k);
                    yield return null;
                }
            }
            ShowOnly(readyPanel);
        }

        void OnReady()
        {
            currentQuestion = -1;
            ShowOnly(quizPanel);
            NextQuestion();
        }

        void NextQuestion()
        {
            currentQuestion++;
            if (feedbackText) feedbackText.text = "";

            if (questions == null || currentQuestion >= questions.Length)
            {
                ShowOnly(endPanel);
                return;
            }

            var q = questions[currentQuestion];
            if (questionText) questionText.text = q.prompt;

            for (int i = 0; i < 3; i++)
            {
                if (optionButtonLabels != null && i < optionButtonLabels.Length && optionButtonLabels[i])
                    optionButtonLabels[i].text = q.options[i];
            }

            waitingForAnswer = true;
        }

        void OnOption(int index)
        {
            if (!waitingForAnswer) return;
            waitingForAnswer = false;

            var q = questions[currentQuestion];
            bool correct = (index == q.correctIndex);

            if (correct)
            {
                if (feedbackText)
                {
                    feedbackText.color = new Color(0.2f, 0.8f, 0.2f);
                    feedbackText.text = "Amazing, that’s right!";
                }
                if (audioSource && correctClip) audioSource.PlayOneShot(correctClip);
                StartCoroutine(NextAfterDelay(1.1f));
            }
            else
            {
                if (feedbackText)
                {
                    feedbackText.color = new Color(0.9f, 0.2f, 0.2f);
                    feedbackText.text = "Oops, try again!";
                }
                if (audioSource && wrongClip) audioSource.PlayOneShot(wrongClip);
                StartCoroutine(AllowRetry());
            }
        }

        IEnumerator AllowRetry()
        {
            yield return new WaitForSeconds(0.8f);
            if (feedbackText) feedbackText.text = "";
            waitingForAnswer = true;
        }

        IEnumerator NextAfterDelay(float s)
        {
            yield return new WaitForSeconds(s);
            NextQuestion();
        }
    }
}
