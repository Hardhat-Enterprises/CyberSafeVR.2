using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace InsiderThreat01 {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class StickyNote : MonoBehaviour
    {
        [Header("Return-to-Start")]
        [SerializeField] float returnDuration = 0.5f;
        [SerializeField] AnimationCurve returnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        Rigidbody rb;
        XRGrabInteractable grab;
        Vector3 startPos;
        Quaternion startRot;
        Transform startParent;

        bool isInBin = false;
        bool isReturning = false;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            grab = GetComponent<XRGrabInteractable>();
        }

        void OnEnable()
        {
            // Record spawn transform the first time we enable
            startPos = transform.position;
            startRot = transform.rotation;
            startParent = transform.parent;

            grab.selectExited.AddListener(OnDropped);
        }

        void OnDisable()
        {
            grab.selectExited.RemoveListener(OnDropped);
        }

        void OnDropped(SelectExitEventArgs _)
        {
            // If it wasn’t snapped into the bin, fly back home
            if (!isInBin && gameObject.activeInHierarchy)
                StartCoroutine(ReturnHome());
        }

        IEnumerator ReturnHome()
        {
            if (isReturning) yield break;
            isReturning = true;

            // Temporarily make it kinematic while we tween back
            bool prevKinematic = rb.isKinematic;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 fromPos = transform.position;
            Quaternion fromRot = transform.rotation;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / returnDuration;
                float k = returnCurve.Evaluate(Mathf.Clamp01(t));
                transform.position = Vector3.Lerp(fromPos, startPos, k);
                transform.rotation = Quaternion.Slerp(fromRot, startRot, k);
                yield return null;
            }

            transform.SetPositionAndRotation(startPos, startRot);
            transform.SetParent(startParent, true);

            rb.isKinematic = prevKinematic;
            isReturning = false;
        }

        /// <summary>
        /// Called by DustbinZone when the note enters its trigger.
        /// Snaps the note into the bin and disables grabbing.
        /// </summary>
        public void SnapIntoBin(Transform snapPoint, ParticleSystem fireFx)
        {
            if (isInBin) return;

            isInBin = true;

            // If currently held, disabling the grab component cleanly releases it
            grab.enabled = false;

            // Make physics stable
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Snap & parent
            transform.SetParent(snapPoint, worldPositionStays: false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // Fire!
            if (fireFx != null)
            {
                if (!fireFx.gameObject.activeInHierarchy) fireFx.gameObject.SetActive(true);
                fireFx.Play();
            }
        }
    }
}

