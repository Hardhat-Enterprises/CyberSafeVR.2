using System.Collections.Generic;
using UnityEngine;

namespace InsiderThreat02
{
    /// <summary>
    /// Manages the audit process, tracking inspected objects and calculating scores.
    /// </summary>

    public class AuditManager : MonoBehaviour
    {
        public List<Interactable> allObjects = new List<Interactable>(); // All objects in scene
        private int inspectedCount = 0;

        private UIManager uiManager;

        public void ObjectInspected(Interactable obj)
        {
            if (!allObjects.Contains(obj))
            {
                allObjects.Add(obj);
                inspectedCount++;
                Debug.Log("Inspected: " + obj.title);
            }
        }

        public void ShowAuditScore()
        {
            int totalObjects = allObjects.Count;
            string message = $"You inspected {inspectedCount} out of {totalObjects} objects.";

            Debug.Log(message);

            if (uiManager != null)
            {
                uiManager.ShowInfo("Audit Complete", message, "safe");
            }
        }
    }
}   