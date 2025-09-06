using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SWB01
{
    public class SocketLabelHandler : MonoBehaviour
    {
        public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;       // The socket itself
        public string answerName;               // Name of the bool in AnswerHandler (e.g., "URL", "Encrypt")
        public float ejectForce = 1f;           // Upward force when ejecting

        private string notSafeName;
        private string safeName;
        public bool isSafe;

        // Reference to the AnswerHandler
        public AnswerHandler answerHandler;

        // Remember the last placed object
        private GameObject lastPlacedObject = null;

        private void Start()
        {
            if (socket == null)
                socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();

            if (socket != null)
                socket.selectEntered.AddListener(OnObjectPlaced);

            if (answerHandler == null)
                Debug.LogWarning("AnswerHandler not assigned on " + gameObject.name);

            notSafeName = answerName + "NotSafe";
            safeName = answerName + "Safe";
        }

        private void OnObjectPlaced(SelectEnterEventArgs args)
        {
            GameObject attachedObject = args.interactableObject.transform.gameObject;
            string objectName = attachedObject.name;

            Transform parent = transform.parent;

            if (parent == null || parent.childCount < 4)
            {
                Debug.LogWarning("Socket does not have enough siblings for activation!");
                return;
            }

            if (objectName == safeName)
            {
                parent.GetChild(2).gameObject.SetActive(true);
                isSafe = true;
            }
            else if (objectName == notSafeName)
            {
                parent.GetChild(1).gameObject.SetActive(true);
                isSafe = false;
            }
            else
            {
                EjectObject();
                return;
            }

            // Update AnswerHandler
            if (answerHandler != null && !string.IsNullOrEmpty(answerName))
            {
                answerHandler.SetAnswer(answerName, isSafe);
                answerHandler.IncrementTotal();
            }

            // Remember this object
            lastPlacedObject = attachedObject;

            // Deactivate the placed object
            attachedObject.SetActive(false);

            // Disable the socket itself
            socket.enabled = false;
        }

        public void OnObjectRemoved()
        {
            if (answerHandler != null){
                answerHandler.DecrementTotal();
                answerHandler.SetAnswer(answerName, false);
            }

            if (lastPlacedObject != null)
            {
                // Reactivate the last placed object
                lastPlacedObject.SetActive(true);

                // Call Respawn() from RespawnOnFloorHit
                RespawnOnFloorHit respawnComponent = lastPlacedObject.GetComponent<RespawnOnFloorHit>();
                if (respawnComponent != null)
                    respawnComponent.Respawn();

                lastPlacedObject = null; // Clear reference
            }
        }

        public void EjectObject()
        {
            if (socket == null || !socket.hasSelection || socket.interactablesSelected.Count == 0)
                return;

            UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable = socket.interactablesSelected[0];
            socket.interactionManager.SelectExit(socket, interactable);

            Debug.Log("Ejected object: " + interactable.transform.name);
        }

    }
}
