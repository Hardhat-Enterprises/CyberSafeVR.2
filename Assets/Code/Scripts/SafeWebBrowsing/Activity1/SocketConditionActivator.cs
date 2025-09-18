using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace SWB01
{
    public class SocketConditionActivator : MonoBehaviour
    {
        public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor[] sockets; // Assign your 3 sockets in the inspector
        public UnityEvent onAllSocketsFilled; // Events to invoke once all sockets are filled

        public GameManager gameManager;
        private bool hasTriggered = false; // Ensures events only fire once

        void Start()
        {
            // Subscribe to socket events instead of using periodic checking
            foreach (var socket in sockets)
            {
                socket.selectEntered.AddListener(OnItemPlacedInSocket);
                socket.selectExited.AddListener(OnItemRemovedFromSocket);
            }
        }

        void OnDestroy()
        {
            // Unsubscribe from events when destroyed
            foreach (var socket in sockets)
            {
                socket.selectEntered.RemoveListener(OnItemPlacedInSocket);
                socket.selectExited.RemoveListener(OnItemRemovedFromSocket);
            }
        }

        void OnItemPlacedInSocket(SelectEnterEventArgs args)
        {
            CheckSockets();
        }

        void OnItemRemovedFromSocket(SelectExitEventArgs args)
        {
            // Reset the triggered state if an item is removed
            hasTriggered = false;
        }

        void Reset()
        {
            hasTriggered = false;
        }

        void CheckSockets()
        {
            Debug.Log("checking");
            if (hasTriggered) return; // Already triggered, do nothing

            bool allFilled = true;

            foreach (var socket in sockets)
            {
                if (!socket.hasSelection)
                {
                    allFilled = false;
                    break;
                }
            }

            if (allFilled)
            {
                onAllSocketsFilled.Invoke(); // Trigger events assigned in inspector
                hasTriggered = true;
            }
        }

    }
}