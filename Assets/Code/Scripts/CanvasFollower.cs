using UnityEngine;

public class CanvasFollower : MonoBehaviour
{
    public Transform target;        // usually the XR camera
    public float distance = 2.0f;   // distance in front of camera
    public float heightOffset = -0.3f; // slight vertical offset (optional)
    public float followSpeed = 5f;

    void Update()
    {
        if (target == null) return;

        Vector3 forward = target.forward;
        Vector3 targetPosition = target.position + forward * distance;
        targetPosition.y += heightOffset;

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // Always face the user
        transform.rotation = Quaternion.LookRotation(transform.position - target.position);
    }
}
