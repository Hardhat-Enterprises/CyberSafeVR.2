using UnityEngine;


namespace CATM_WV
{
    public class SpecificObjectSocket : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
    {
        // Updated to use the new interface
        public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
        {
            // Only accept objects with SpiderController
            if (interactable.transform.GetComponent<SpiderController>() != null)
                return base.CanSelect(interactable);

            return false;
        }
    }
}
