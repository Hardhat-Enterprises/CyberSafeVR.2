using UnityEngine;
using PS01;

public class SimpleDoorOpener : MonoBehaviour
{
    public Transform door;           // Reference to the door (child object)
    public float openAngle = 90f;    // How far the door opens
    public float rotateSpeed = 2f;   // How fast the door opens

    private bool openForward = true;
    private Quaternion targetRotation;
    private Quaternion originalRotation;

    void Start()
    {
        if (door == null)
        {
            Debug.LogError("Door reference is not set!");
            return;
        }

        originalRotation = door.rotation;
        targetRotation = originalRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (door == null) return;

        openForward = !openForward;
        float angle = openForward ? openAngle : -openAngle;

        // Rotate around the Y axis relative to the original rotation
        targetRotation = Quaternion.Euler(0f, angle, 0f) * originalRotation;
    }

    void Update()
    {
        if (door == null) return;

        // Smoothly rotate the door toward the target
        door.rotation = Quaternion.Slerp(door.rotation, targetRotation, Time.deltaTime * rotateSpeed);
    }
}
