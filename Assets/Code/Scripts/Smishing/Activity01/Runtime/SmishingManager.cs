using System.Collections;
using UnityEngine;

namespace Smishing01
{
    /// <summary>
    /// Core orchestrator. Runs scenarios in sequence.
    /// </summary>
    public class SmishingManager : MonoBehaviour
    {
        [Header("Data")]
        public SmishingScenarioData[] scenarios;

        [Header("Components (auto-resolved at runtime if null)")]
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

        [Header("Debug")]
        [SerializeField] private bool verboseLogging = true;

        private int  _scenarioIndex;
        private bool _started;

        // Per-scenario answer state — set by phone callback, read by coroutine
        private bool         _answered;
        private PlayerAction _answeredAction;

        private void Start()
        {
            AutoResolveRefs();

            if (scenarios == null || scenarios.Length == 0)
            {
                scenarios = Resources.FindObjectsOfTypeAll<SmishingScenarioData>();
                if (scenarios != null && scenarios.Length > 1)
                    System.Array.Sort(scenarios, (a, b) =>
                        string.Compare(a.name, b.name, System.StringComparison.Ordinal));
            }

            if (!ValidateRequired()) return;

            // Subscribe ONCE to the phone's event
            phoneUI.OnPlayerAction += OnPlayerAnswered;

            progressTracker.Initialise(scenarios.Length);
            hud.BuildDots(scenarios.Length);
            hud.SetScore(0, scenarios.Length);
            hud.ShowNarration("Smishing Awareness", "Put on your investigator hat.");

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

        private void OnDestroy()
        {
            if (phoneUI != null)
                phoneUI.OnPlayerAction -= OnPlayerAnswered;
        }

        private void OnPlayerAnswered(PlayerAction action)
        {
            if (verboseLogging)
                Debug.Log($"[SmishingManager] Player answered: {action} (scenario index {_scenarioIndex})");
            _answeredAction = action;
            _answered       = true;
        }

        private void OnBeginPressed()
        {
            if (_started) return;
            _started = true;
            StartCoroutine(RunScenarios());
        }

        private IEnumerator RunScenarios()
        {
            yield return new WaitForSeconds(delayAfterIntro);

            while (_scenarioIndex < scenarios.Length)
            {
                var data = scenarios[_scenarioIndex];

                if (verboseLogging)
                    Debug.Log($"[SmishingManager] === Starting scenario {_scenarioIndex + 1}/{scenarios.Length}: {data.scenarioTitle} ===");

                // 1. Update HUD
                hud.ShowNarration(data.scenarioTitle, data.introNarration);
                hud.SetCurrentStep(_scenarioIndex);
                if (audioController != null) audioController.PlayMessage();

                yield return new WaitForSeconds(delayBetweenStages);

                // 2. Reset answer state, present phone, wait
                _answered = false;
                phoneUI.DisplayScenario(data);

                yield return new WaitUntil(() => _answered);

                bool correct      = (_answeredAction == data.correctAction);
                bool hintUsedThis = phoneUI.WasHintUsed;

                if (verboseLogging)
                    Debug.Log($"[SmishingManager] Answer: {_answeredAction}, expected: {data.correctAction}, correct: {correct}");

                // 3. Reveal red-flag highlight, brief pause
                phoneUI.RevealRedFlags();
                yield return new WaitForSeconds(highlightRevealDelay);

                phoneUI.Hide();

                // 4. Audio + score
                if (audioController != null)
                {
                    if (correct) audioController.PlayCorrect();
                    else         audioController.PlayIncorrect();
                }

                progressTracker.RecordAnswer(correct, hintUsedThis);
                hud.MarkStepResult(_scenarioIndex, correct);
                hud.SetScore(progressTracker.CurrentScore, scenarios.Length);

                // 5. Feedback panel — wait for Continue
                bool feedbackDone = false;
                outcomeHandler.ShowOutcomeAndWait(correct, data, () => feedbackDone = true);
                yield return new WaitUntil(() => feedbackDone);

                if (verboseLogging)
                    Debug.Log($"[SmishingManager] Continue clicked, advancing to next scenario");

                _scenarioIndex++;
            }

            if (verboseLogging) Debug.Log("[SmishingManager] All scenarios complete, loading debrief.");
            outcomeHandler.LoadDebrief();
        }

        private void AutoResolveRefs()
        {
            if (introScreen     == null) introScreen     = FindFirstObjectByType<IntroScreenController>(FindObjectsInactive.Include);
            if (phoneUI         == null) phoneUI         = FindFirstObjectByType<PhoneMessageUI>(FindObjectsInactive.Include);
            if (hud             == null) hud             = FindFirstObjectByType<HUDController>(FindObjectsInactive.Include);
            if (outcomeHandler  == null) outcomeHandler  = FindFirstObjectByType<ScenarioOutcomeHandler>(FindObjectsInactive.Include);
            if (progressTracker == null) progressTracker = FindFirstObjectByType<ScenarioProgressTracker>(FindObjectsInactive.Include);
            if (audioController == null) audioController = FindFirstObjectByType<ScenarioAudioController>(FindObjectsInactive.Include);
            if (progressTracker == null) progressTracker = gameObject.AddComponent<ScenarioProgressTracker>();
        }

        private bool ValidateRequired()
        {
            bool ok = true;
            if (scenarios == null || scenarios.Length == 0)
            { Debug.LogError("[SmishingManager] No scenarios assigned. Run CyberSafeVR → Smishing → Build Complete Smishing01 Scene."); ok = false; }
            if (phoneUI == null)
            { Debug.LogError("[SmishingManager] PhoneMessageUI missing."); ok = false; }
            if (hud == null)
            { Debug.LogError("[SmishingManager] HUDController missing."); ok = false; }
            if (outcomeHandler == null)
            { Debug.LogError("[SmishingManager] ScenarioOutcomeHandler missing."); ok = false; }
            return ok;
        }
    }
}
