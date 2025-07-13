using UnityEngine;

namespace SWB01
{
    public class SocketConditionActivator: MonoBehaviour
    {
        public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor[] sockets; // Assign your 3 sockets here in the inspector
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable buttonInteractable;

        void Start()
        {
            buttonInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            if (buttonInteractable != null)
                buttonInteractable.enabled = false; // Disable at start

            InvokeRepeating(nameof(CheckSockets), 0f, 0.5f); // Check periodically
        }

        void CheckSockets()
        {
            bool allFilled = true;

            foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket in sockets)
            {
                if (!socket.hasSelection)
                {
                    allFilled = false;
                    break;
                }
            }

            if (buttonInteractable != null)
                buttonInteractable.enabled = allFilled;
        }
    }
}