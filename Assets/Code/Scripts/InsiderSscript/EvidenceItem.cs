using UnityEngine;

namespace InsiderThreat02
{
    /// <summary>
    /// ScriptableObject representing an evidence item in the training module.
    /// </summary>

    [CreateAssetMenu(menuName = "Training/Evidence Item")]
    public class EvidenceItem : ScriptableObject
    {
        public string evidenceId;
        public string displayName;
        [TextArea] public string description;
        public bool suspicious;
    }
}
