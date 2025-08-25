using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleCanvasOnSelect : MonoBehaviour
{
    [SerializeField] GameObject targetCanvas;

    public void OnSelected(SelectEnterEventArgs _)
    {
        if (targetCanvas) targetCanvas.SetActive(!targetCanvas.activeSelf);
    }
}
