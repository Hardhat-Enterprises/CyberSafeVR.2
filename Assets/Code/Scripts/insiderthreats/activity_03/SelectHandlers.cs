using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InsiderThreat03
{
    // ─── CollectEvidenceOnSelect ─────────────────────────────────────────────
    // Attach to any interactable prop. Hook to XRSimpleInteractable > Select Entered.
    //
    // BUG FIX: original used InsiderThreat02 namespace — this is Activity 03,
    // so namespace must be InsiderThreat03. Mixing namespaces across activities
    // causes class resolution ambiguity and silently broken event wiring.

    /// <summary>
    /// Collects an EvidenceItem when this object is selected by the VR controller.
    ///
    /// PREFAB WIRING: assign the matching EvidenceItem ScriptableObject asset to
    /// the 'evidence' slot in the Inspector. If this slot is empty the collect
    /// call is silently skipped — the original code had no warning for this.
    /// </summary>
    public class CollectEvidenceOnSelect : MonoBehaviour
    {
        [SerializeField] EvidenceItem evidence;

        void Start()
        {
            // BUG FIX: surface missing ScriptableObject assignment at startup.
            if (evidence == null)
                Debug.LogWarning($"[CollectEvidenceOnSelect:{name}] 'evidence' is not " +
                    "assigned. Create an EvidenceItem asset and assign it in Inspector.");
        }

        public void OnSelected(SelectEnterEventArgs _)
        {
            if (evidence == null) return;

            if (EvidenceManager.Instance == null)
            {
                Debug.LogError("[CollectEvidenceOnSelect] EvidenceManager singleton not " +
                    "found. Make sure EvidenceManager is present in the scene.");
                return;
            }

            EvidenceManager.Instance.Collect(evidence);
        }
    }


    // ─── ToggleCanvasOnSelect ─────────────────────────────────────────────────
    // BUG FIX: same namespace correction (InsiderThreat02 → InsiderThreat03).
    // Added null-safety log so missing targetCanvas wiring is immediately visible.

    /// <summary>
    /// Toggles a world-space canvas on/off when the object is VR-selected.
    ///
    /// PREFAB WIRING: assign the canvas GameObject (e.g. the InfoPanel) to the
    /// 'targetCanvas' slot. This is one of the prefab slots flagged as missing
    /// in PR #82 — it must be assigned per-prop in the scene Hierarchy.
    /// </summary>
    public class ToggleCanvasOnSelect : MonoBehaviour
    {
        [SerializeField] GameObject targetCanvas;

        void Start()
        {
            if (targetCanvas == null)
                Debug.LogWarning($"[ToggleCanvasOnSelect:{name}] 'targetCanvas' is not " +
                    "assigned. Drag the relevant canvas into this slot in the Inspector.");
        }

        public void OnSelected(SelectEnterEventArgs _)
        {
            if (targetCanvas == null)
            {
                Debug.LogError($"[ToggleCanvasOnSelect:{name}] Cannot toggle — " +
                    "targetCanvas is null.");
                return;
            }
            targetCanvas.SetActive(!targetCanvas.activeSelf);
        }
    }
}