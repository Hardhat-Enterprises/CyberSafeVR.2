using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 3f; // how far the player can interact
    public LayerMask interactableLayer; // assign in Inspector (e.g., "Interactable")

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // press E to interact (or replace with VR button)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                if (interactable != null)
                {
                    Debug.Log("Interacting with: " + hit.collider.name);
                    interactable.OnSelect();
                }
            }
        }
    }
}
