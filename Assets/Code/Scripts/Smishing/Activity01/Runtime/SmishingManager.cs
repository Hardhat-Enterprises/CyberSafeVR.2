using System.Collections;
using UnityEngine;

namespace Smishing01
{
    /// <summary>
    /// Core orchestrator.
    /// Flow:
    ///   1. Show intro/briefing screen
    ///   2. Wait for player to press Begin
    ///   3. For each scenario:
    ///      a. Show phone with message + hint button
    ///      b. Wait for player response (Report / Ignore / Click Link)
    ///      c. Reveal red-flag highlight on the phone
    ///      d. Show correct/incorrect feedback panel
    ///      e. Wait for Continue button
    ///   4. Load the Debrief scene
    /// </summary>
    public class SmishingManager : MonoBehaviour
    {
        [Header("Data")]
        public SmishingScenarioData[] scenarios;

        [Header("Components (auto-wired by builder)")]
        public IntroScreenController  introScreen;
        public PhoneMessageUI         phoneUI;
        public HUDController          hud;
        public ScenarioOutcomeHandler outcomeHandler;
        public ScenarioProgressTracker progressTracker;
        public ScenarioAudioController audioController;

        [Header("Timing")]
        [SerializeField] private float delayAfterIntro      = 0.3f;
        [SerializeField] private float highlightRevealDelay = 1.5f;
        [SerializeField] private float delayBetweenStages   = 0.5f;

        private int _scenarioIndex;
        private bool _started;

        // ── Unity lifecycle ───────────────────────────────────────────────────

        private void Start()
        {
            if (scenarios == null || scenarios.Length == 0)
            {
                Debug.LogError("[SmishingManager] No scenarios assigned!");
                return;
            }

            progressTracker.Initialise(scenarios.Length);
            hud.BuildDots(scenarios.Length);
            hud.SetScore(0, scenarios.Length);
            hud.ShowNarration("Smishing Awareness", "Put on your investigator hat.");

            // Show intro, wait for Begin
            if (introScreen != null)
            {
                introScreen.OnStartRequested += OnBeginPressed;
                introScreen.Show();
            }
            else
            {
                OnBeginPressed();
            }
        }

        private void OnBeginPressed()
        {
            if (_started) return;
            _started = true;
            StartCoroutine(RunScenarios());
        }

        // ── Main loop ────────────────────────────────────────────────────────

        private IEnumerator RunScenarios()
        {
            yield return new WaitForSeconds(delayAfterIntro);

            while (_scenarioIndex < scenarios.Length)
            {
                var data = scenarios[_scenarioIndex];

                // 1. Update HUD
                hud.ShowNarration(data.scenarioTitle, data.introNarration);
                hud.SetCurrentStep(_scenarioIndex);
                audioController?.PlayMessage();

                yield return new WaitForSeconds(delayBetweenStages);

                // 2. Present the phone and wait for input
                bool   answered = false;
                bool   correct  = false;

                System.Action<PlayerAction> handler = null;
                handler = (action) =>
                {
                    correct  = (action == data.correctAction);
                    answered = true;
                    phoneUI.OnPlayerAction -= handler;
                };
                phoneUI.OnPlayerAction += handler;

                phoneUI.DisplayScenario(data);

                yield return new WaitUntil(() => answered);

                bool hintUsedThis = phoneUI.WasHintUsed;

                // 3. Reveal red-flag highlight, brief pause so player sees it
                phoneUI.RevealRedFlags();
                yield return new WaitForSeconds(highlightRevealDelay);

                phoneUI.Hide();

                // 4. Audio + score tracking
                if (correct) audioController?.PlayCorrect();
                else         audioController?.PlayIncorrect();

                progressTracker.RecordAnswer(correct, hintUsedThis);
                hud.MarkStepResult(_scenarioIndex, correct);
                hud.SetScore(progressTracker.CurrentScore, scenarios.Length);

                // 5. Feedback panel — waits for Continue
                bool feedbackDone = false;
                outcomeHandler.ShowOutcomeAndWait(correct, data, () => feedbackDone = true);
                yield return new WaitUntil(() => feedbackDone);

                _scenarioIndex++;
            }

            // Done — load debrief
            outcomeHandler.LoadDebrief();
        }
    }
}
