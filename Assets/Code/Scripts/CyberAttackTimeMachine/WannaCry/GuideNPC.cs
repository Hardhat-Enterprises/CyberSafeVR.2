using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace CATM_WC{
    public class GuideNPC : MonoBehaviour
    {
        public Transform playerTarget;
        public UnityEvent onFacedPlayer;

        private Quaternion originalRotation; // Store the original rotation

        void Start()
        {
            originalRotation = transform.rotation; // Save starting rotation
        }

        public void FacePlayer()
        {
            StartCoroutine(FaceTarget(playerTarget, true));
        }

        IEnumerator FaceTarget(Transform target, bool invokeEvent)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude < 0.01f)
                yield break;

            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            yield return StartCoroutine(RotateTo(targetRotation, 0.5f));

            if (invokeEvent && onFacedPlayer != null)
                onFacedPlayer.Invoke();
        }

        IEnumerator RotateTo(Quaternion targetRotation, float angleThreshold)
        {
            float rotationSpeed = 90f; // degrees per second

            while (Quaternion.Angle(transform.rotation, targetRotation) > angleThreshold)
            {
                Debug.Log("rotating");
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null;
            }

            transform.rotation = targetRotation;
        }
    }
}