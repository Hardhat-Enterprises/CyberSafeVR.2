using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

namespace CATM_WV
{
    [RequireComponent(typeof(Animator), typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
    public class SpiderController : MonoBehaviour
    {
        [Header("Movement Points")]
        public Transform pointA;
        public Transform pointB;

        [Header("Movement Settings")]
        public float moveSpeed = 0.5f;
        public float waitAtPoint = 0.2f; // optional pause at each point

        private Vector3 target;
        private Animator animator;
        private bool placed = false;
        private bool flipping = false;

        void Awake()
        {
            animator = GetComponent<Animator>();
            target = pointB.position;

            // Make Rigidbody kinematic to avoid physics issues
            Rigidbody rb = GetComponent<Rigidbody>();
            Debug.Log("SpiderController enabled");
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }


        void Update()
        {
            if (placed || flipping) return;

            // Move manually toward target
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            // Play walking animation
            animator.Play("Walk");

            // Check if reached target
            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                StartCoroutine(FlipAtPoint());
            }
        }

        private IEnumerator FlipAtPoint()
        {
            flipping = true;

            // Snap to target
            transform.position = target;

            // Optional wait
            if (waitAtPoint > 0)
                yield return new WaitForSeconds(waitAtPoint);

            // Switch target
            target = (target == pointA.position) ? pointB.position : pointA.position;

            // Flip 180° around Y
            transform.Rotate(0f, 180f, 0f);

            flipping = false;
        }


        public void StartIdle()
        {
            placed = true;
            animator.Play("Idle");    
        }
    }
}
