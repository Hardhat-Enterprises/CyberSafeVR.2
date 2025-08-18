using UnityEngine;

namespace CATM_WC
{
    public class LockTransform : MonoBehaviour
    {
        public Transform targetTransform; // The transform to lock to

        void LateUpdate()
        {
            if (targetTransform != null)
            {
                transform.position = targetTransform.position;
                transform.rotation = targetTransform.rotation;
            }
        }

        public void SetTarget(Transform newTarget)
        {
            targetTransform = newTarget;
        }
    }
}
