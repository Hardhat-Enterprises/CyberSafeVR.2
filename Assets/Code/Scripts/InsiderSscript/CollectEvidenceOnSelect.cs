using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InsiderThreat02
{
    /// <summary>
    /// Collects evidence when the object is selected in a VR environment.
    /// </summary>

    public class CollectEvidenceOnSelect : MonoBehaviour
    {
        [SerializeField] EvidenceItem evidence;

        public void OnSelected(SelectEnterEventArgs _)
        {
            if (evidence) EvidenceManager.Instance?.Collect(evidence);
        }
    }
}
