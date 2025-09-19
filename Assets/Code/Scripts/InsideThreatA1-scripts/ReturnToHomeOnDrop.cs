using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace InsiderThreat01 {
    [RequireComponent(typeof(XRGrabInteractable))]
    public class ReturnToHomeOnDrop : MonoBehaviour
    {
        private XRGrabInteractable grabInteractable;
        private Vector3 startPosition;
        private Quaternion startRotation;

        [Header("Optional Home Point")]
        [Tooltip("Leave empty to use the starting transform of this object")]
        public Transform homeTransform;

        void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();

            // Store starting transform if no homeTransform is given
            if (homeTransform == null)
            {
                startPosition = transform.position;
                startRotation = transform.rotation;
            }

            // Listen for drop event
            grabInteractable.selectExited.AddListener(OnDrop);
        }

        void Start()
        {
            // On game start, force the object into the home position
            ResetToHome();
        }

        private void OnDrop(SelectExitEventArgs args)
        {
            // If object was placed in a socket, socket will hold it → do nothing
            if (grabInteractable.isSelected) return;

            // Otherwise return to home
            ResetToHome();
        }

        private void ResetToHome()
        {
            if (homeTransform != null)
            {
                transform.SetPositionAndRotation(homeTransform.position, homeTransform.rotation);
            }
            else
            {
                transform.SetPositionAndRotation(startPosition, startRotation);
            }

            // Clear leftover velocity from Rigidbody
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}

