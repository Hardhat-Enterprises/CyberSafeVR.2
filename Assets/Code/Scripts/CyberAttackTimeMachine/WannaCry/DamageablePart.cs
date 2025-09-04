using UnityEngine;
using UnityEngine.Events;

namespace CATM_WV
{
    public class DamageablePart : MonoBehaviour
    {
        [Header("Repair Settings")]
        public float repairTime = 2f; // Time the hammer must touch the part to repair
        public GameObject sparkEffect; // Assign your sparks particle system here

        [Header("Events")]
        public UnityEvent onRepaired; // Event triggered when repair is complete

        private float currentRepairProgress = 0f;

        void Start()
        {
            if (sparkEffect != null)
                sparkEffect.SetActive(true);
        }

        public void RepairStep(float deltaTime)
        {

            currentRepairProgress += deltaTime;

            if (currentRepairProgress >= repairTime)
            {
                CompleteRepair();
            }
        }

        private void CompleteRepair()
        {

            Debug.Log($"{gameObject.name} has been repaired!");
            // Trigger the event
            onRepaired?.Invoke();
            transform.parent.gameObject.SetActive(false);
        }
    }
}
