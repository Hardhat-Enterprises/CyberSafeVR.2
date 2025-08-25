using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleInspectable : MonoBehaviour
{
    [SerializeField] string itemName = "Item";
    [TextArea, SerializeField] string infoMessage = "Info...";
    [SerializeField] bool isSuspicious = false;

    // Hook this to XR Simple Interactable -> Select Entered
    public void OnSelected(SelectEnterEventArgs _)
    {
        UIManager.Instance?.ShowInfo(itemName, infoMessage, isSuspicious);
    }
}
