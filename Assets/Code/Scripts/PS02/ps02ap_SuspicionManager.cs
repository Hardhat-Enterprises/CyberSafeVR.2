using UnityEngine;
using UnityEngine.UI;

public class SuspicionManager : MonoBehaviour
{
    public Slider suspicionSlider;
    public float maxSuspicion = 100f;
    public float currentSuspicion;

    public GameObject winEndCanvas;
    public GameObject loseEndCanvas;

    private void Start()
    {
        currentSuspicion = 0f;
        suspicionSlider.maxValue = maxSuspicion;
        suspicionSlider.value = currentSuspicion;

        // Decrease frustration by 4 every 1 seconds
        InvokeRepeating(nameof(DecreaseAutomatically), 1f, 1f);
    }

    public void IncreaseSuspicion(float amount)
    {
        currentSuspicion += amount;

        // Optional: snap to 100f if we're super close due to float quirks
        if (currentSuspicion > maxSuspicion - 0.01f)
        {
            currentSuspicion = maxSuspicion;
        }

        currentSuspicion = Mathf.Clamp(currentSuspicion, 0, maxSuspicion);
        suspicionSlider.value = Mathf.Round(currentSuspicion * 100f) / 100f;

        if (currentSuspicion >= maxSuspicion)
        {
            TriggerInvestigation();
        }
    }

    public void IncreaseByThree()
    {
        IncreaseSuspicion(3f);
    }

    public void DecreaseSuspicion(float amount)
    {
        currentSuspicion -= amount;
        currentSuspicion = Mathf.Clamp(currentSuspicion, 0, maxSuspicion);
        suspicionSlider.value = currentSuspicion;
    }

    private void DecreaseAutomatically()
    {
        // Only decrease if neither end canvas is active
        if ((winEndCanvas != null && winEndCanvas.activeSelf) ||
            (loseEndCanvas != null && loseEndCanvas.activeSelf))
        {
            return;
        }

        DecreaseSuspicion(4f); // decrease by 4 every 1 seconds
    }

    private void TriggerInvestigation()
    {
        suspicionSlider.value = maxSuspicion;
        Debug.Log("Suspicion maxed! Investigation triggered!");
        // Add any consequence visuals/audio here
    }
}