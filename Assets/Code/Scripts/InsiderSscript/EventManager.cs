using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InsiderThreat02
{
    /// <summary>  
    /// Manages the collection and tracking of evidence items.
    /// </summary>
    /// 

    public class EvidenceManager : MonoBehaviour
    {
        public static EvidenceManager Instance { get; private set; }
        public List<EvidenceItem> collected = new();

        [Header("UI")]
        [SerializeField] GameObject listPanel;
        [SerializeField] Transform listRoot;
        [SerializeField] GameObject listRowPrefab; // a simple prefab: Text for name + (optional) icon

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Collect(EvidenceItem item)
        {
            if (item == null || collected.Contains(item)) return;
            collected.Add(item);
            AddRow(item);
        }

        void AddRow(EvidenceItem item)
        {
            if (!listRoot || !listRowPrefab) return;
            var row = Instantiate(listRowPrefab, listRoot);
            var texts = row.GetComponentsInChildren<Text>();
            if (texts.Length > 0) texts[0].text = item.displayName;
        }

        public int SuspiciousCount()
        {
            int c = 0;
            foreach (var e in collected) if (e.suspicious) c++;
            return c;
        }
    }
}
