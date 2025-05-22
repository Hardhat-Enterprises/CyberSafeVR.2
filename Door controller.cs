using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openAngle = 90f;
    public float speed = 2f;
    private bool isOpen = false;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            isOpen ? openRotation : closedRotation,
            Time.deltaTime * speed
        );
    }
}
