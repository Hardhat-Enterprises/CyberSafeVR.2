using UnityEngine;
 
namespace InsiderThreat03
{
    [CreateAssetMenu(menuName = "Training/Evidence Item (IT-03)")]
    public class EvidenceItem : ScriptableObject
    {
        public string evidenceId;
        public string displayName;
        [TextArea] public string description;
        public bool suspicious;
    }
}