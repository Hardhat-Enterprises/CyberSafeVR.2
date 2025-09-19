using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace SMS02
{
    public class TeleportOnClick : MonoBehaviour
    {
        public Transform teleportTarget; // Assign PhoneSpawnPoint
        public GameObject xrOrigin;      // Assign XR Origin in inspector

        public void TeleportPlayer()
        {
            if (teleportTarget != null && xrOrigin != null)
            {
                // Move XR Origin to target position
                xrOrigin.transform.position = teleportTarget.position;
                xrOrigin.transform.rotation = teleportTarget.rotation;
            }
        }
    }
}
