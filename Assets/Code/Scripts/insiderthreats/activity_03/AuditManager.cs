using System.Collections.Generic;
using UnityEngine;

namespace InsiderThreat03
{
    /// <summary>
    /// Manages the audit process for Activity 03 (WorkstationComparison),
    /// tracking inspected objects and calculating scores.
    /// </summary>
    public class AuditManager : MonoBehaviour
    {
        public List<Interactable> allObjects = new List<Interactable>();

        // BUG FIX: was declared private with no initialisation — uiManager was
        // ALWAYS null, so ShowAuditScore() never displayed anything in-game.
        // Now exposed as [SerializeField] so it can be wired up in the prefab
        // Inspector, with a fallback FindFirstObjectByType for safety.
        [SerializeField] private UIManager uiManager;

        private int inspectedCount = 0;

        void Awake()
        {
            if (uiManager == null)
            {
                uiManager = Object.FindFirstObjectByType<UIManager>();
                if (uiManager == null)
                    Debug.LogWarning("[AuditManager] UIManager not found. " +
                        "Assign it in the Inspector on the AuditManager prefab.");
            }
        }

        public void ObjectInspected(Interactable obj)
        {
            if (!allObjects.Contains(obj))
            {
                allObjects.Add(obj);
                inspectedCount++;
                Debug.Log($"[AuditManager] Inspected: {obj.title} " +
                    $"({inspectedCount}/{allObjects.Count})");
            }
        }

        public void ShowAuditScore()
        {
            int totalObjects = allObjects.Count;
            string message = $"You inspected {inspectedCount} out of {totalObjects} objects.";

            Debug.Log($"[AuditManager] {message}");

            // BUG FIX: previously this block could never execute because
            // uiManager was never assigned. Now it works.
            if (uiManager != null)
            {
                uiManager.ShowInfo("Audit Complete", message, "safe");
            }
            else
            {
                Debug.LogError("[AuditManager] Cannot show score — UIManager is missing.");
            }
        }
    }
}