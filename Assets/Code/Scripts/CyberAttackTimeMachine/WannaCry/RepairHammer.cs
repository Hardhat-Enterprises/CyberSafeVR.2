using UnityEngine;

namespace CATM_WV
{
    public class RepairHammer : MonoBehaviour
    {
        [Header("Hammer Motion Settings")]
        public float swingSpeed = 5f;
        public float swingAngle = 20f;

        private bool isHammering = false;
        private float swingTimer = 0f;
        private Quaternion initialRotation;

        void Awake()
        {
            initialRotation = transform.localRotation;
        }

        void Update()
        {
            if (isHammering)
            {
                swingTimer += Time.deltaTime * swingSpeed;
                float angle = Mathf.Sin(swingTimer) * swingAngle;

                // Swing whole hammer back and forth around local X axis
                transform.localRotation = initialRotation * Quaternion.Euler(0f, angle, 0f);
            }
        }

        public void StartHammering()
        {
            isHammering = true;
            swingTimer = 0f;
        }

        public void StopHammering()
        {
            isHammering = false;
            transform.localRotation = initialRotation; // reset
        }

        void OnTriggerStay(Collider other)
        {
            DamageablePart part = other.GetComponent<DamageablePart>();
            if (part != null)
            {
                // Keep repairing the same way
                part.RepairStep(Time.deltaTime);
            }
        }
    }
}
