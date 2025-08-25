using UnityEngine;

[CreateAssetMenu(menuName = "Training/Evidence Item")]
public class EvidenceItem : ScriptableObject
{
    public string evidenceId;
    public string displayName;
    [TextArea] public string description;
    public bool suspicious;
}
