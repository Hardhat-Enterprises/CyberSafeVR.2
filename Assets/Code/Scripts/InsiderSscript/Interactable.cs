using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Object Info")]
    public string title;
    [TextArea] public string body;
    public string status; // "safe", "risky", "warning"

    private UIManager uiManager;
    public AuditManager auditManager;

    void Start()
    {
        // Recommended Unity 2023 LTS method
        uiManager = Object.FindFirstObjectByType<UIManager>();

        if (uiManager == null)
            Debug.LogWarning("UIManager not found in scene!");
    }

    /// <summary>
    /// Call this when the player selects/inspects the object
    /// </summary>
    public void OnSelect()
    {
        if (uiManager != null)
            uiManager.ShowInfo(title, body, status);

        if (auditManager != null)
            auditManager.ObjectInspected(this);
    }
}