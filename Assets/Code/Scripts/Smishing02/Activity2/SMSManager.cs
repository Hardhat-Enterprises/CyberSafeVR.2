using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SMS02
{
    public class SMSManager : MonoBehaviour
    {
        [System.Serializable]
        public class SMSMessage
        {
            public string text;
            public bool isSmishing;
        }

        [Header("Panels")]
        public GameObject introPanel;
        public GameObject instructionsPanel;

        [Header("UI Elements")]
        public TextMeshProUGUI phoneText;
        public Button smishingButton;
        public Button safeButton;
        public Button continueButton;
        public TextMeshProUGUI resultText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI streakText;
        public Slider progressBar;

        [Header("Messages")]
        public SMSMessage[] messages;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip correctClip;
        public AudioClip incorrectClip;

        [Header("Lighting")]
        public Light feedbackLight;
        public Color correctColor = Color.green;
        public Color incorrectColor = Color.red;
        private Color defaultColor;

        [Header("Teleport Settings")]
        public Transform spawnPoint;   // assign empty GameObject in next room
        public GameObject player;      // assign XR Origin or Main Camera Rig

        private int currentIndex = 0;
        private int score = 0;
        private int streak = 0;
        private bool hasAttemptedWrong = false;
        private bool awaitingCorrectAnswer = false;

        void Start()
        {
            smishingButton.onClick.AddListener(() => EvaluateMessage(true));
            safeButton.onClick.AddListener(() => EvaluateMessage(false));
            continueButton.onClick.AddListener(GoToNextRoom);

            smishingButton.gameObject.SetActive(false);
            safeButton.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(false);
            instructionsPanel.SetActive(false);

            if (feedbackLight != null)
                defaultColor = feedbackLight.color;
        }

        public void ShowInstructions()
        {
            introPanel?.SetActive(false);
            instructionsPanel?.SetActive(true);
        }

        public void StartGame()
        {
            instructionsPanel?.SetActive(false);

            currentIndex = 0;
            score = 0;
            streak = 0;
            hasAttemptedWrong = false;
            awaitingCorrectAnswer = true;

            smishingButton.gameObject.SetActive(true);
            safeButton.gameObject.SetActive(true);
            continueButton.gameObject.SetActive(false);

            DisplayMessage();
            UpdateScoreDisplay();
            UpdateStreakDisplay();

            Invoke(nameof(AllowInput), 0.5f);
        }

        void AllowInput() => awaitingCorrectAnswer = false;

        void DisplayMessage()
        {
            if (currentIndex < messages.Length)
            {
                phoneText.text = messages[currentIndex].text;
                resultText.text = "";
                hasAttemptedWrong = false;
                awaitingCorrectAnswer = false;
                UpdateProgressBar();
            }
            else
            {
                EndGame();
            }
        }

        void EvaluateMessage(bool userSaysSmishing)
        {
            if (awaitingCorrectAnswer) return;

            bool correct = messages[currentIndex].isSmishing == userSaysSmishing;

            if (correct)
            {
                int points = (!hasAttemptedWrong) ? (streak >= 3 ? 15 : 10) : 0;
                score += points;
                streak++;

                resultText.text = "Correct! Good job.";
                resultText.color = Color.green;
                PlaySound(correctClip);

                UpdateScoreDisplay();
                UpdateStreakDisplay();
                StartCoroutine(FlashLight(correctColor));

                awaitingCorrectAnswer = true;
                Invoke(nameof(NextMessage), 2f);
            }
            else
            {
                resultText.text = "Incorrect! Try Again.";
                resultText.color = Color.red;
                PlaySound(incorrectClip);

                streak = 0;
                hasAttemptedWrong = true;

                UpdateStreakDisplay();
                StartCoroutine(FlashLight(incorrectColor));
            }
        }

        void NextMessage()
        {
            if (awaitingCorrectAnswer)
            {
                currentIndex++;
                DisplayMessage();
            }
        }

        void EndGame()
        {
            phoneText.text = "All messages complete!";
            resultText.text = "Final Score: " + score;

            smishingButton.gameObject.SetActive(false);
            safeButton.gameObject.SetActive(false);
            continueButton.gameObject.SetActive(true);

            if (progressBar != null)
                progressBar.value = 1f;
        }

        public void GoToNextRoom()
        {
            if (spawnPoint != null && player != null)
            {
                player.transform.position = spawnPoint.position;
                player.transform.rotation = spawnPoint.rotation;
            }

            continueButton?.gameObject.SetActive(false);
        }

        void UpdateScoreDisplay() => scoreText.text = "Score: " + score;

        void UpdateStreakDisplay()
        {
            if (streakText != null)
            {
                if (streak >= 3)
                    streakText.text = $"Wohoo!!! Streak {streak}!";
                else
                    streakText.text = $"Streak: {streak}";
            }
        }

        void UpdateProgressBar()
        {
            if (progressBar != null && messages.Length > 0)
                progressBar.value = (float)currentIndex / messages.Length;
        }

        void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
                audioSource.PlayOneShot(clip, 0.5f);
        }

        IEnumerator FlashLight(Color color)
        {
            if (feedbackLight != null)
            {
                feedbackLight.color = color;
                yield return new WaitForSeconds(0.5f);
                feedbackLight.color = defaultColor;
            }
        }
    }
}
