using UnityEngine;

namespace SWB01{
    public class RespawnOnFloorHit : MonoBehaviour
    {
        private Vector3 originalPosition;
        private Quaternion originalRotation;

        [Tooltip("Tag of the floor collider that will trigger the respawn")]
        public string floorTag = "Floor";

        void Start()
        {
            // Save the original position and rotation at the start
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(floorTag))
            {
                Respawn();
            }
        }


        private void Respawn()
        {
            // Reset position and rotation
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            // Reset velocity if it has a Rigidbody
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}

