using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SWB01{
    public class NamedSocketInteractor : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
    {
        [Header("Allowed Object Name")]
        public string allowedName; // The name of the object this socket accepts

        public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
        {
            // Check base conditions first (so it respects normal socket rules)
            if (!base.CanSelect(interactable))
                return false;

            // Only allow if the object's GameObject name matches
            return interactable.transform.name == allowedName;
        }

        // Hover filter (controls hover mesh)
        public override bool CanHover(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable interactable)
        {
            if (!base.CanHover(interactable))
                return false;

            return interactable.transform.name == allowedName;
        }
    }
}
