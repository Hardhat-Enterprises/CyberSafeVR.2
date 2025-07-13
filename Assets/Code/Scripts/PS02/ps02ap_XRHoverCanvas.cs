using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRHoverCanvas : MonoBehaviour
{
    public GameObject canvasToShow;

    private void Awake()
    {
        if (canvasToShow != null)
            canvasToShow.SetActive(false);
    }

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (canvasToShow != null)
        {
            canvasToShow.SetActive(true);
        }
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        if (canvasToShow != null)
        {
            canvasToShow.SetActive(false);
        }
    }
}
