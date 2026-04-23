using System;
using UnityEngine;

namespace Smishing01
{
    /// <summary>
    /// Tracks the player's score across scenarios and persists it to PlayerPrefs.
    /// </summary>
    public class ScenarioProgressTracker : MonoBehaviour
    {
        private const string KEY_SCORE     = "Smishing01_Score";
        private const string KEY_TOTAL     = "Smishing01_Total";
        private const string KEY_COMPLETED = "Smishing01_Completed";
        private const string KEY_HINTS     = "Smishing01_HintsUsed";

        public int CurrentScore   { get; private set; }
        public int TotalScenarios { get; private set; }
        public int CurrentStep    { get; private set; }
        public int HintsUsed      { get; private set; }

        public event Action<int, int, int> OnProgressUpdated; // score, step, total

        public void Initialise(int totalScenarios)
        {
            TotalScenarios = totalScenarios;
            CurrentScore   = 0;
            CurrentStep    = 0;
            HintsUsed      = 0;

            PlayerPrefs.SetInt(KEY_TOTAL,     totalScenarios);
            PlayerPrefs.SetInt(KEY_SCORE,     0);
            PlayerPrefs.SetInt(KEY_COMPLETED, 0);
            PlayerPrefs.SetInt(KEY_HINTS,     0);
            PlayerPrefs.Save();
        }

        public void RecordAnswer(bool correct, bool usedHint)
        {
            CurrentStep++;
            if (correct)  CurrentScore++;
            if (usedHint) HintsUsed++;

            PlayerPrefs.SetInt(KEY_SCORE,     CurrentScore);
            PlayerPrefs.SetInt(KEY_COMPLETED, CurrentStep);
            PlayerPrefs.SetInt(KEY_HINTS,     HintsUsed);
            PlayerPrefs.Save();

            OnProgressUpdated?.Invoke(CurrentScore, CurrentStep, TotalScenarios);
        }

        public bool IsComplete() => CurrentStep >= TotalScenarios;
    }
}
