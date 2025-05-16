using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReceivePassword : MonoBehaviour
{
    [Header("Movement Settings")]
    public float openX = 0.01f;         // Target X position when opening
    public float closedX = -0.1974f;     // Target X position when closing
    public float moveSpeed = 2f;         // How fast it moves

    private bool shouldOpen = false;
    private bool hasPlacedObject = false;
    private Transform myTransform;
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;

    private void Start()
    {
        myTransform = transform;
        socketInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();

        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnSelectEntered);
        }
    }

    private void Update()
    {
        // Decide the target position based on if it should open
        float targetX = shouldOpen ? openX : closedX;

        // Smoothly move towards the target X position
        Vector3 localPos = myTransform.localPosition;
        localPos.x = Mathf.Lerp(localPos.x, targetX, Time.deltaTime * moveSpeed);
        myTransform.localPosition = localPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlacedObject) return; 
        if (other.CompareTag("PasswordPaper"))
        {
            shouldOpen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PasswordPaper"))
        {
            shouldOpen = false;
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // When the socket actually selects (grabs) the object
        shouldOpen = false; // Start closing
        hasPlacedObject = true;
    }

    private void OnDestroy()
    {
        // Always good to unsubscribe to avoid errors
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnSelectEntered);
        }
    }
}
