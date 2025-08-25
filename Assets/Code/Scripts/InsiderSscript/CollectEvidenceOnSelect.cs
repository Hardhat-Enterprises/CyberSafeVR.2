using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CollectEvidenceOnSelect : MonoBehaviour
{
    [SerializeField] EvidenceItem evidence;

    public void OnSelected(SelectEnterEventArgs _)
    {
        if (evidence) EvidenceManager.Instance?.Collect(evidence);
    }
}
