using UnityEngine;

namespace PS02
{
    public class TeleportPlayer : MonoBehaviour
    {
        public Transform xrRig; // Assign XR Rig
        public Transform teleportSpot; // Where to teleport to

        public void Teleport()
        {
            if (xrRig != null && teleportSpot != null)
            {
                xrRig.position = teleportSpot.position;
                xrRig.rotation = teleportSpot.rotation;
            }
        }
    }
}