using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LidController : MonoBehaviour
{
    [Header("Lid References")]
    [SerializeField] private Transform lidVisual; // The visual mesh part that should rotate
    [SerializeField] private float openAngle = -45f; // Negative angle to rotate downward
    [SerializeField] private float lidOpenSpeed = 2f;
    [SerializeField] private float lidCloseSpeed = 1f;
    
    private Quaternion lidClosedRotation;
    private Quaternion lidOpenRotation;
    private bool lidShouldOpen = false;
    
    // Track papers that are touching the lid
    private List<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable> papersInContact = new List<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    
    void Start()
    {
        // If lidVisual not assigned, use this object (which should be the lid mesh)
        if (lidVisual == null)
        {
            lidVisual = transform;
        }
        
        // Store initial lid rotation
        lidClosedRotation = lidVisual.localRotation;
        
        // Calculate open rotation - NEGATIVE angle for downward movement
        Vector3 openEuler = lidVisual.localRotation.eulerAngles;
        openEuler.x += openAngle; // Add negative angle to rotate downward
        lidOpenRotation = Quaternion.Euler(openEuler);
        
        // Ensure collider is set to trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.Log("Set collider to trigger mode");
        }
    }
    
    void Update()
    {
        // Handle lid visual rotation
        if (lidShouldOpen)
        {
            lidVisual.localRotation = Quaternion.Slerp(lidVisual.localRotation, lidOpenRotation, Time.deltaTime * lidOpenSpeed);
        }
        else
        {
            lidVisual.localRotation = Quaternion.Slerp(lidVisual.localRotation, lidClosedRotation, Time.deltaTime * lidCloseSpeed);
        }
        
        // Check for released papers
        CheckForReleasedPapers();
    }
    
    private void CheckForReleasedPapers()
    {
        // Use a copy to safely iterate and remove items
        List<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable> papersToCheck = new List<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(papersInContact);
        
        foreach (var grabInteractable in papersToCheck)
        {
            if (grabInteractable == null)
            {
                papersInContact.Remove(grabInteractable);
                continue;
            }
            
            // If paper is no longer being held, destroy it
            if (!grabInteractable.isSelected)
            {
                papersInContact.Remove(grabInteractable);
                Destroy(grabInteractable.gameObject);
                Debug.Log("Paper destroyed after release");
            }
            
            if (papersInContact.Count == 0)
            {
                lidShouldOpen = false;
                Debug.Log("Closing lid");
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if it's paper
        if (other.CompareTag("Paper"))
        {
            // Open the lid
            lidShouldOpen = true;
            Debug.Log("Paper detected - opening lid");
            
            // Track the paper object
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null && !papersInContact.Contains(grabInteractable))
            {
                papersInContact.Add(grabInteractable);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Check if it's paper
        if (other.CompareTag("Paper"))
        {
            // Remove from tracking
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = other.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabInteractable != null)
            {
                papersInContact.Remove(grabInteractable);
            }
            
            // Close the lid if no papers are touching it
            if (papersInContact.Count == 0)
            {
                lidShouldOpen = false;
                Debug.Log("No more papers - closing lid");
            }
        }
    }
}