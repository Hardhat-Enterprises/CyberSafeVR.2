using UnityEngine;
using UnityEngine.UI;

public class FrustrationManager : MonoBehaviour
{
    public Slider frustrationSlider;
    public float maxFrustration = 100f; //Set the maximum frustration level
    public float currentFrustration;

    public GameObject winEndCanvas;
    public GameObject loseEndCanvas;

    private void Start()
    {
        currentFrustration = 0f;
        frustrationSlider.maxValue = maxFrustration;
        frustrationSlider.value = currentFrustration;

        // Decrease frustration by 3 every 5 seconds
        InvokeRepeating(nameof(DecreaseAutomatically), 1f, 1f);
    }

    public void IncreaseFrustration(float amount)
    {
        currentFrustration += amount;

        // Snap to 100f if we're super close due to float quirks
        if (currentFrustration > maxFrustration - 0.01f)
        {
            currentFrustration = maxFrustration;
        }

        currentFrustration = Mathf.Clamp(currentFrustration, 0, maxFrustration);
        frustrationSlider.value = Mathf.Round(currentFrustration * 100f) / 100f;

        if (currentFrustration >= maxFrustration)
        {
            TriggerBreakdown();
        }
    }

    public void IncreaseByFive()
    {
        IncreaseFrustration(5f);
    }
    public void DecreaseFrustration(float amount)
    {
        currentFrustration -= amount;
        currentFrustration = Mathf.Clamp(currentFrustration, 0, maxFrustration);
        frustrationSlider.value = currentFrustration;
    }

    private void DecreaseAutomatically()
    {
        // Only decrease if neither end canvas is active
        if ((winEndCanvas != null && winEndCanvas.activeSelf) ||
            (loseEndCanvas != null && loseEndCanvas.activeSelf))
        {
            return;
        }

        DecreaseFrustration(2f); // decrease by 1 every 5 seconds
    }

    private void TriggerBreakdown()
    {
        frustrationSlider.value = maxFrustration; // force visual bar to fill
        Debug.Log("User is overwhelmed! Triggering event...");
        // You can add red screen flash, audio scream, etc.
    }
}
