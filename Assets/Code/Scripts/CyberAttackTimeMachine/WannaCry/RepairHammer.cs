using UnityEngine;

namespace CATM_WV
{
    public class RepairHammer : MonoBehaviour
    {
        void OnTriggerStay(Collider other)
        {
            DamageablePart part = other.GetComponent<DamageablePart>();
            if (part != null)
            {
                // Increment repair progress while the hammer stays on the part
                part.RepairStep(Time.deltaTime);
            }
        }
    }
}
