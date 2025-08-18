using UnityEngine;
using System.Collections;

namespace SWB01
{
    public class NPCGuide : MonoBehaviour
    {
        private Animator animator;
        public GameObject poofPrefab;
        public LockTransform lockTransform;

        [Header("Teleport Targets")]
        public Transform locationA; // First location
        public Transform locationB; // Second location

        private Transform currentLocation; // Tracks where NPC currently is
        private bool isTeleporting = false;

        void Awake()
        {
            animator = GetComponent<Animator>();
            animator.applyRootMotion = false;

        }

        public void startRoll()
        {
            animator.SetBool("Roll_Anim", true);
        }

        public void stopRoll()
        {
            animator.SetBool("Roll_Anim", false);
        }

        // Generic teleport routine
        private IEnumerator TeleportRoutine(Transform target)
        {
            
            isTeleporting = true;

            startRoll();
            
            yield return new WaitForSeconds(1f);

            if (poofPrefab != null)
            {
                Instantiate(poofPrefab, transform.position, Quaternion.identity);
            }

            lockTransform.TeleportTo(target.position, target.eulerAngles);

            if (poofPrefab != null)
            {
                Instantiate(poofPrefab, transform.position, Quaternion.identity);
            }

            stopRoll();

            currentLocation = target;
            isTeleporting = false;
        }

        // Call this to teleport to Location A
        public void GoToLocationA()
        {
            if (!isTeleporting && currentLocation != locationA)
            {
                StartCoroutine(TeleportRoutine(locationA));
            }
        }

        // Call this to teleport to Location B
        public void GoToLocationB()
        {
            Debug.Log("Attempting Teleport to B");
            if (!isTeleporting && currentLocation != locationB)
            {
                StartCoroutine(TeleportRoutine(locationB));
            }
        }
    }
}
