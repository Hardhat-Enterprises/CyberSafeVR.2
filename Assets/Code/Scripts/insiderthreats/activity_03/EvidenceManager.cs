using System.Collections.Generic;
using UnityEngine;
using TMPro; // BUG FIX: legacy UnityEngine.UI.Text is deprecated in Unity 6.
             // Use TextMeshProUGUI. Requires TextMeshPro package (already in project).

namespace InsiderThreat03
{
    // BUG FIX 1: This file was named EventManager.cs but contains EvidenceManager.
    // Rename the file to EvidenceManager.cs to match the class name and avoid
    // confusion with Unity's own EventManager pattern.
    //
    // BUG FIX 2: listRowPrefab was a SerializeField with no prefab assigned in the
    // scene — one of the "missing prefabs" flagged in PR #82. See PREFAB SETUP below.

    /// <summary>
    /// Singleton that tracks collected evidence items and populates the UI list.
    /// Attach to a persistent GameObject in the InsiderThreat03 scene.
    ///
    /// PREFAB SETUP (fixes the missing prefab error):
    ///   1. Create a UI GameObject: Canvas > Panel > Vertical Layout Group
    ///   2. Inside it add a child called "ListRow": Text (TMP) + optional Image
    ///   3. Drag that child into Assets/Prefabs/InsiderThreat/Activity03/
    ///      and name it "EvidenceListRow.prefab"
    ///   4. Assign EvidenceListRow.prefab to the listRowPrefab slot on this component.
    ///   5. Assign the panel GameObject to listPanel, its Transform to listRoot.
    /// </summary>
    public class EvidenceManager : MonoBehaviour
    {
        public static EvidenceManager Instance { get; private set; }
        public List<EvidenceItem> collected = new();

        [Header("UI — assign in Inspector (fixes missing prefab)")]
        [SerializeField] GameObject listPanel;
        [SerializeField] Transform listRoot;

        // BUG FIX: This prefab was missing from the scene — flagged in PR #82.
        // Create Assets/Prefabs/InsiderThreat/Activity03/EvidenceListRow.prefab
        // (a simple GameObject with a TextMeshProUGUI component) and assign it here.
        [SerializeField] GameObject listRowPrefab;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Validate required prefab wiring at startup to surface the bug early.
            if (listRowPrefab == null)
                Debug.LogError("[EvidenceManager] listRowPrefab is not assigned! " +
                    "Create EvidenceListRow.prefab and assign it in the Inspector.");

            if (listRoot == null)
                Debug.LogError("[EvidenceManager] listRoot Transform is not assigned.");
        }

        public void Collect(EvidenceItem item)
        {
            if (item == null || collected.Contains(item)) return;
            collected.Add(item);
            AddRow(item);
        }

        void AddRow(EvidenceItem item)
        {
            // BUG FIX: added early-out guard with error log instead of silent failure.
            if (listRoot == null || listRowPrefab == null)
            {
                Debug.LogError("[EvidenceManager] Cannot add row — prefab or root missing.");
                return;
            }

            var row = Instantiate(listRowPrefab, listRoot);

            // BUG FIX: was using UnityEngine.UI.Text (deprecated/missing in Unity 6 URP).
            // Now uses TextMeshProUGUI, which is already present in the project.
            var tmp = row.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
                tmp.text = item.displayName;
            else
                Debug.LogWarning($"[EvidenceManager] EvidenceListRow prefab has no " +
                    $"TextMeshProUGUI child — displayName '{item.displayName}' won't show.");
        }

        public int SuspiciousCount()
        {
            int c = 0;
            foreach (var e in collected) if (e.suspicious) c++;
            return c;
        }
    }
}