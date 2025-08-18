using UnityEngine;

namespace SWB01
{
    public class SingleObjectSocketFilter : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
    {
        public GameObject allowedObject; // assign in Inspector

        public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
        {
            // Only allow the specific object
            return interactable.transform.gameObject == allowedObject && base.CanSelect(interactable);
        }
    }
}
