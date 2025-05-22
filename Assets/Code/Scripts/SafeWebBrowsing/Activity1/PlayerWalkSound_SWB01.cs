using UnityEngine;

public class PlayerWalkSound_SWB01 : MonoBehaviour
{
    public AudioSource audioSource; // Assign the AudioSource in the Inspector
    public AudioClip walkingSound;  // Assign the walking sound in the Inspector
    public float walkingSpeedThreshold = 0.1f; // Threshold for walking speed (adjust as needed)

    private Vector3 lastPosition;
    private bool isWalking = false;

    void Start()
    {
        lastPosition = transform.position; // Store the initial position of the XR Rig
    }

    void Update()
    {
        // Calculate the distance moved by comparing the current position to the last position
        Vector3 movement = transform.position - lastPosition;
        float movementDistance = movement.magnitude;

        // Check if the movement exceeds the threshold for walking
        bool shouldPlaySound = movementDistance > walkingSpeedThreshold;

        // If the player is moving, and the sound isn't already playing, start the walking sound
        if (shouldPlaySound && !isWalking)
        {
            isWalking = true;
            audioSource.PlayOneShot(walkingSound); // Play the walking sound
        }
        // If the player stops moving, stop the walking sound
        else if (!shouldPlaySound && isWalking)
        {
            isWalking = false;
            audioSource.Stop(); // Stop the sound immediately
        }

        // Update the last position for the next frame
        lastPosition = transform.position;
    }
}
