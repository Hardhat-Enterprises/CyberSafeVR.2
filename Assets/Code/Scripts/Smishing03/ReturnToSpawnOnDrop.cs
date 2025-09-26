using UnityEngine;

namespace Smishing03
{
    public class ReturnToSpawnOnDrop : MonoBehaviour
    {
        Vector3 startPos; Quaternion startRot; Rigidbody rb;

        void Awake()
        {
            startPos = transform.position;
            startRot = transform.rotation;
            rb = GetComponent<Rigidbody>();
        }

        public void ResetToSpawn()
        {
            if (!rb) rb = GetComponent<Rigidbody>();
            if (rb)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            transform.SetPositionAndRotation(startPos, startRot);
        }
    }
}
