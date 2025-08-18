using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CATM_WC
{
    public class NPCGuide : MonoBehaviour
    {
        private Animator animator;
        public GameObject poofPrefab;
        public LockTransform lockTransform;

        [Header("Teleport Targets")]
        public List<Transform> locations = new List<Transform>();

        private Transform currentLocation;
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

        private IEnumerator TeleportRoutine(Transform target)
        {
            isTeleporting = true;
            startRoll();

            yield return new WaitForSeconds(2f);

            // Poof at the old location
            if (poofPrefab != null)
                Instantiate(poofPrefab, transform.position, Quaternion.identity);

            // Assign the new target to LockTransform so NPC follows it
            if (lockTransform != null)
                lockTransform.SetTarget(target);

            // Poof at the new location
            if (poofPrefab != null)
                Instantiate(poofPrefab, target.position, Quaternion.identity);

            stopRoll();
            currentLocation = target;
            isTeleporting = false;
        }

        public void GoToLocation(int index)
        {
            if (index < 0 || index >= locations.Count)
            {
                Debug.LogWarning($"Location index {index} is out of range.");
                return;
            }

            Transform target = locations[index];
            if (!isTeleporting && currentLocation != target)
            {
                StartCoroutine(TeleportRoutine(target));
            }
        }
    }
}
