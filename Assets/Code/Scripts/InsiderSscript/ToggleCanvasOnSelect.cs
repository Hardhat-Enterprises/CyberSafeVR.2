using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InsiderThreat02
{
    /// <summary>
    /// Toggles a specified canvas on or off when the object is selected.
    /// </summary>

    public class ToggleCanvasOnSelect : MonoBehaviour
    {
        [SerializeField] GameObject targetCanvas;

        public void OnSelected(SelectEnterEventArgs _)
        {
            if (targetCanvas) targetCanvas.SetActive(!targetCanvas.activeSelf);
        }
    }
}
