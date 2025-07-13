using UnityEngine;

[ExecuteAlways]
public class DayNightCycle : MonoBehaviour
{
    [Tooltip("Length of one full day-night cycle in real seconds.")]
    [SerializeField]
    private float dayDurationInSeconds = 120f; // Default: 2 minutes

    private void Update()
    {
        if (dayDurationInSeconds <= 0f)
            return;

        // Calculate rotation per frame
        float degreesPerSecond = 360f / dayDurationInSeconds;
        float rotationThisFrame = degreesPerSecond * Time.deltaTime;

        // Rotate around the X-axis to simulate sun movement
        transform.Rotate(Vector3.right, rotationThisFrame);
    }
}
