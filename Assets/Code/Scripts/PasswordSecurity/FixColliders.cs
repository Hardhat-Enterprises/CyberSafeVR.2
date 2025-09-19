using UnityEngine;
using UnityEditor;

public class RebindColliders : MonoBehaviour
{
    [MenuItem("Tools/XR/Rebind Colliders to Self")]
    static void RebindAllColliders()
    {
        int fixedCount = 0;

        // Find all Interactables in the scene
        var interactables = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>(true);

        foreach (var interactable in interactables)
        {
            // Get all colliders on this GameObject only
            var ownColliders = interactable.GetComponents<Collider>();
            if (ownColliders.Length == 0)
            {
                Debug.LogWarning($"[XR Fix] No colliders found on {interactable.name}");
                continue;
            }

            // Assign only own colliders
            interactable.colliders.Clear();
            foreach (var col in ownColliders)
            {
                interactable.colliders.Add(col);
            }

            fixedCount++;
        }

        Debug.Log($"[XR Fix] Reassigned self-colliders for {fixedCount} interactables.");
    }
}
