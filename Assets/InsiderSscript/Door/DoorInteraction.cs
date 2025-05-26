using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 2f;
    public bool isOpen = false;

    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private Coroutine _currentCoroutine;

    void Start()
    {
        _closedRotation = transform.rotation;
        _openRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0f, openAngle, 0f)); // Rotate on Y-axis
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            _currentCoroutine = StartCoroutine(ToggleDoor());
        }
    }

    private System.Collections.IEnumerator ToggleDoor()
    {
        Quaternion targetRotation = isOpen ? _closedRotation : _openRotation;
        isOpen = !isOpen;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}
