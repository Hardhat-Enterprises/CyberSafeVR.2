using UnityEngine;
using UnityEngine.UI;

namespace InsiderThreat02
{
    /// <summary>
    /// Handles the player's verdict choice and displays the result based on collected evidence.
    /// </summary>

    public class VerdictController : MonoBehaviour
    {
        [SerializeField] Text resultText;
        [SerializeField] int suspiciousThreshold = 2;

        public void ChooseInnocent()
        {
            var susp = EvidenceManager.Instance?.SuspiciousCount() ?? 0;
            if (resultText) resultText.text = susp == 0 ? "Innocent ✅" : "Inconclusive — review evidence.";
        }

        public void ChooseGuilty()
        {
            var susp = EvidenceManager.Instance?.SuspiciousCount() ?? 0;
            if (resultText) resultText.text = susp >= suspiciousThreshold ? "Guilty ✅" : "Inconclusive — more evidence needed.";
        }
    }
}
