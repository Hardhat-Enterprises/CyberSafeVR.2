using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

namespace SSA01
{
    public class TeleportManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerTransform; // Assign this in inspector or find via tag
        [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationProvider teleportationProvider;


        private void Start()
        {
            if (playerTransform == null)
            {
                Debug.LogError("No player found. Teleporation won't work.");
            }
            if(teleportationProvider == null)
            {
                Debug.LogError("No teleporation provider found.");
            }
            
        }


        public void Teleport(Transform iPosition)
        {
            if (playerTransform == null || teleportationProvider == null) return;

            // Create the teleportation request to sit down
            UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportRequest teleportRequest = new UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportRequest
            {
                destinationPosition = iPosition.position,
                destinationRotation = iPosition.rotation
            };

            // Queue the teleportation request to move the player to the sitting position
            teleportationProvider.QueueTeleportRequest(teleportRequest);
        }

    }
}
