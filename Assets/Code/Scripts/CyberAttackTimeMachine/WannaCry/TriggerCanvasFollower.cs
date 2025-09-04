using UnityEngine;
namespace CATM_WC
{
    public class TriggerCanvasFollower : MonoBehaviour
    {
        public Transform target; // usually the XR camera
        public float orbitSpeed = 8f; // how fast to move around parent
        private float horizontalDistance; // The horizontal distance to maintain
        private float heightOffset; // Height difference from NPC
        private Transform pivot;
        
        void Start()
        {
            pivot = transform.parent;
            if (pivot == null)
            {
                Debug.LogError("CanvasFollower needs a parent pivot.");
                enabled = false;
                return;
            }
            
            // Calculate HORIZONTAL distance only (ignore Y difference)
            Vector3 horizontalOffset = transform.position - pivot.position;
            horizontalOffset.y = 0f; // ignore height for distance calculation
            horizontalDistance = horizontalOffset.magnitude;
            
            // Store height offset
            heightOffset = transform.position.y - pivot.position.y;
        }
        
        void Update()
        {
            if (target == null) return;
            
            // Direction from NPC to player (horizontal only)
            Vector3 directionToPlayer = target.position - pivot.position;
            directionToPlayer.y = 0f; // ignore height
            
            if (directionToPlayer.magnitude < 0.001f) return; // avoid zero division
            
            directionToPlayer.Normalize();
            
            // Place canvas at EXACT horizontal distance in direction of player
            Vector3 newPosition = pivot.position + (directionToPlayer * horizontalDistance);
            newPosition.y = pivot.position.y + heightOffset; // set exact height
            
            // Move to new position
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * orbitSpeed);
            
            // Face the camera
            transform.rotation = Quaternion.LookRotation(transform.position - target.position);
        }
    }
}