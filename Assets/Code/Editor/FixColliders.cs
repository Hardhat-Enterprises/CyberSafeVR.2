using UnityEngine;
using UnityEditor;


public class FixColliders : MonoBehaviour
{
    [MenuItem("Tools/XR/Rebind Colliders to Self")]
    static void RebindAllColliders()
    {
        int fixedCount = 0;

        // Use the new API to get all XRBaseInteractables in the scene
        var interactables = Object.FindObjectsByType<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>(FindObjectsSortMode.None);

        foreach (var interactable in interactables)
        {
            // Get colliders directly on this GameObject
            var ownColliders = interactable.GetComponents<Collider>();

            if (ownColliders.Length == 0)continue;
            
            // Rebind only self-colliders
            interactable.colliders.Clear();
            foreach (var col in ownColliders)
            {
                interactable.colliders.Add(col);
            }

            // Mark object as dirty to ensure Unity saves it
            EditorUtility.SetDirty(interactable);

            fixedCount++;
        }

        Debug.Log($"[XR Fix] Reassigned self-colliders for {fixedCount} interactables.");
    }
}
