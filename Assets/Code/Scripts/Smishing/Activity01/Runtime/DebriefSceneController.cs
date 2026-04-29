using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Smishing01
{
    /// <summary>
    /// Controls the Smishing01_End scene. Reads scoring info from PlayerPrefs
    /// and displays a rich results summary.
    /// </summary>
    public class DebriefSceneController : MonoBehaviour
    {
        private const string KEY_SCORE = "Smishing01_Score";
        private const string KEY_TOTAL = "Smishing01_Total";
        private const string KEY_HINTS = "Smishing01_HintsUsed";

        [Header("Text")]
        public TMP_Text scoreText;
        public TMP_Text gradeText;
        public TMP_Text hintsText;

        [Header("Nav Buttons")]
        public Button retryButton;
        public Button menuButton;
        public Button nextButton;

        [Header("Scenes")]
        [SerializeField] private string mainMenuScene   = "MainMenu";
        [SerializeField] private string nextModuleScene = "PasswordSecurity01";
        [SerializeField] private string retryScene      = "Smishing01";

        [Header("Pass threshold %")]
        [SerializeField] private int passPercent = 67;

        private void Start()
        {
            int correct = PlayerPrefs.GetInt(KEY_SCORE, 0);
            int total   = PlayerPrefs.GetInt(KEY_TOTAL, 6);
            int hints   = PlayerPrefs.GetInt(KEY_HINTS, 0);
            int pct     = total > 0 ? Mathf.RoundToInt(correct / (float)total * 100f) : 0;

            if (scoreText != null) scoreText.text = $"{correct} / {total}  ({pct}%)";
            if (hintsText != null) hintsText.text = hints == 0 ? "No hints used 🏆" : $"Hints used: {hints}";

            if (gradeText != null)
                gradeText.text = pct >= passPercent
                    ? "<color=#4CD97B>✔  Activity Passed</color>"
                    : "<color=#FF6B6B>✘  Keep Practising</color>";

            retryButton?.onClick.AddListener(() => SceneManager.LoadScene(retryScene));
            menuButton ?.onClick.AddListener(() => SceneManager.LoadScene(mainMenuScene));
            nextButton ?.onClick.AddListener(() => SceneManager.LoadScene(nextModuleScene));
        }
    }
}
