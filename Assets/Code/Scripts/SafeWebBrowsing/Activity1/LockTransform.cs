using UnityEngine;

namespace SWB01{
    public class LockTransform : MonoBehaviour
    {
        public Vector3 lockedPosition;
        public Vector3 lockedRotation;


        void LateUpdate()
        {
            transform.position = lockedPosition;
            transform.eulerAngles = lockedRotation;
        }

        public void TeleportTo(Vector3 newPosition, Vector3 newRotation)
        {
            lockedPosition = newPosition;
            lockedRotation = newRotation;
        }
    }
}
