using UnityEngine;

namespace InsiderThreat03
{
    /// <summary>
    /// Base class for all interactable objects in the InsiderThreat03 scene.
    /// Attach to every workstation/prop GameObject, then wire up in the prefab.
    ///
    /// PREFAB WIRING (fixes the missing prefab references in IT-03):
    ///   - title, body, status: fill in the Inspector per-object
    ///   - auditManager: drag the AuditManager GameObject from the scene Hierarchy
    /// </summary>
    public class Interactable : MonoBehaviour
    {
        [Header("Object Info")]
        public string title;
        [TextArea] public string body;
        public string status;  // "safe", "risky", "warning"

        // BUG FIX: was declared 'public AuditManager auditManager' with no
        // [SerializeField] attribute and no Awake/Start wiring — Unity's
        // serialiser can expose public fields but the prefab in the branch had
        // this slot empty, meaning ObjectInspected() was silently never called.
        // Changed to [SerializeField] private to enforce Inspector assignment and
        // added a Start() warning if it's still missing.
        [SerializeField] private AuditManager auditManager;

        private UIManager uiManager;

        void Start()
        {
            uiManager = Object.FindFirstObjectByType<UIManager>();

            if (uiManager == null)
                Debug.LogWarning($"[Interactable:{name}] UIManager not found in scene.");

            // BUG FIX: surface the missing auditManager link at startup.
            if (auditManager == null)
                Debug.LogWarning($"[Interactable:{name}] AuditManager not assigned. " +
                    "Drag the AuditManager from the Hierarchy into this slot.");
        }

        /// <summary>
        /// Called by XR Interaction Toolkit select events or PlayerInteraction.
        /// </summary>
        public void OnSelect()
        {
            if (uiManager != null)
                uiManager.ShowInfo(title, body, status);

            if (auditManager != null)
                auditManager.ObjectInspected(this);
        }
    }
}