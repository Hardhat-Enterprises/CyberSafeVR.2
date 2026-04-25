using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 30f;   // Y-axis spin speed
    public float wobbleAmount = 10f;    // degrees of tilt
    public float wobbleSpeed = 2f;      // wobble frequency

    void Update()
    {
        // Constant rotation around Y
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

        // Wobble on X axis using sine wave
        float wobble = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;

        // Apply wobble while keeping current Y rotation
        transform.rotation = Quaternion.Euler(wobble, transform.eulerAngles.y, 0f);
    }
}