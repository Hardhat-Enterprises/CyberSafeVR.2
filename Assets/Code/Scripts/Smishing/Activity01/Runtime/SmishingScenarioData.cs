using UnityEngine;

namespace Smishing01
{
    public enum PlayerAction { ReportMessage, IgnoreMessage, ClickLink }

    public enum ScenarioCategory
    {
        BankingFraud,
        PackageDelivery,
        GovernmentImpersonation,
        TelcoScam,
        PrizeScam,
        Legitimate
    }

    [CreateAssetMenu(menuName = "CyberSafeVR/Smishing/ScenarioData",
                     fileName  = "NewScenarioData")]
    public class SmishingScenarioData : ScriptableObject
    {
        [Header("Display")]
        public string scenarioTitle   = "Untitled Scenario";
        public ScenarioCategory category = ScenarioCategory.BankingFraud;
        public string senderName      = "Unknown";

        [TextArea(3, 6)]
        public string messageBody     = "";
        public string embeddedUrl     = "";

        [Header("Narration")]
        [TextArea(2, 4)]
        public string introNarration  = "";

        [Header("Answer")]
        public PlayerAction correctAction = PlayerAction.ReportMessage;

        [Header("Feedback")]
        [TextArea(3, 6)]
        public string correctFeedback   = "";
        [TextArea(3, 6)]
        public string incorrectFeedback = "";
        public string[] redFlags        = new string[0];

        [Header("Hint (shown when player uses hint token)")]
        [TextArea(2, 3)]
        public string hintText          = "Look closely at the sender and the URL.";

        [Header("Highlight (substring to highlight red on reveal)")]
        [Tooltip("The suspicious substring inside messageBody or embeddedUrl that will be highlighted after the player answers.")]
        public string suspiciousSubstring = "";

        [Header("Difficulty (1=Easy, 3=Hard)")]
        [Range(1, 3)]
        public int difficultyLevel = 1;
    }
}
