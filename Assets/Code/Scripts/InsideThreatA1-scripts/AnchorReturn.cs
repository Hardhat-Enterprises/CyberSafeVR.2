using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace InsiderThreat01
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class AnchorReturn : MonoBehaviour
    {
        [Header("Anchor")]
        [Tooltip("Leave empty to capture this object's pose at Start.")]
        public Transform anchor;                // desired place
        public bool captureAnchorOnStart = true;

        [Header("Return Behavior")]
        public bool returnOnRelease = true;     // return when dropped (unless in socket)
        public float returnDelay = 0.15f;       // small grace period after release
        public float returnDuration = 0.4f;     // tween time
        public AnimationCurve returnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [Tooltip("If idle and close to the anchor, keep it pinned there by freezing all positions.")]
        public bool pinWhenIdle = true;
        [Tooltip("If object is within this distance of anchor, don't tween (just pin).")]
        public float snapDistance = 0.02f;

        [Header("Gizmos")]
        public Color gizmoColor = new Color(0.2f, 0.8f, 1f, 0.35f);
        public float gizmoSize = 0.05f;

        Rigidbody rb;
        XRGrabInteractable grab;
        RigidbodyConstraints originalConstraints;
        bool held = false;
        bool inSocket = false;
        Coroutine returnCo;

        // Fallback anchor if no Transform is assigned
        Vector3 capturedPos;
        Quaternion capturedRot;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            grab = GetComponent<XRGrabInteractable>();
            originalConstraints = rb.constraints;
        }

        void OnEnable()
        {
            grab.selectEntered.AddListener(OnSelectEntered);
            grab.selectExited.AddListener(OnSelectExited);
        }

        void OnDisable()
        {
            grab.selectEntered.RemoveListener(OnSelectEntered);
            grab.selectExited.RemoveListener(OnSelectExited);
        }

        void Start()
        {
            // Capture current pose as anchor if none was provided
            if (anchor == null && captureAnchorOnStart)
            {
                capturedPos = transform.position;
                capturedRot = transform.rotation;
            }

            // Start pinned at anchor
            if (!held && !inSocket)
            {
                SnapToAnchor();
                if (pinWhenIdle) PinIdle();
            }

            // In case it starts already in a socket, check next frame
            StartCoroutine(DetectSocketNextFrame());
        }

        IEnumerator DetectSocketNextFrame()
        {
            yield return null;
            foreach (var interactor in grab.interactorsSelecting)
            {
                if (interactor is XRSocketInteractor)
                {
                    inSocket = true;
                    break;
                }
            }
        }

        // === Event handlers ===
        void OnSelectEntered(SelectEnterEventArgs args)
        {
            held = true;

            if (args.interactorObject is XRSocketInteractor)
            {
                // Selected BY a socket (placed into socket)
                inSocket = true;
            }

            // Free it while held
            Unpin();
            if (returnCo != null) { StopCoroutine(returnCo); returnCo = null; }
        }

        void OnSelectExited(SelectExitEventArgs args)
        {
            if (args.interactorObject is XRSocketInteractor)
            {
                // Released BY the socket (taken out)
                inSocket = false;
            }

            // If nothing else is selecting us after this frame, we’re not held
            StartCoroutine(PostReleaseCheck());
        }

        IEnumerator PostReleaseCheck()
        {
            yield return null; // wait a frame for any new selection (e.g., socket handoff)
            held = grab.isSelected; // true if now grabbed by something else (e.g., socket)

            if (!held && !inSocket)
            {
                if (returnOnRelease)
                {
                    if (returnCo != null) StopCoroutine(returnCo);
                    returnCo = StartCoroutine(ReturnAfterDelay());
                }
                else
                {
                    // Not returning; just pin where it is
                    if (pinWhenIdle) PinIdle();
                }
            }
        }

        // === Return logic ===
        IEnumerator ReturnAfterDelay()
        {
            if (returnDelay > 0f) yield return new WaitForSeconds(returnDelay);
            if (held || inSocket) yield break;

            Vector3 targetPos = GetAnchorPos();
            Quaternion targetRot = GetAnchorRot();

            // If already close, just snap & pin
            if (Vector3.Distance(transform.position, targetPos) <= snapDistance)
            {
                SnapToAnchor();
                if (pinWhenIdle) PinIdle();
                yield break;
            }

            var prevConstraints = rb.constraints;
            rb.constraints = RigidbodyConstraints.None; // allow clean tween
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 fromPos = transform.position;
            Quaternion fromRot = transform.rotation;

            float t = 0f;
            while (t < 1f)
            {
                if (held || inSocket) break; // abort if grabbed or socketed mid-flight
                t += Time.deltaTime / Mathf.Max(0.01f, returnDuration);
                float k = returnCurve.Evaluate(Mathf.Clamp01(t));
                transform.position = Vector3.Lerp(fromPos, targetPos, k);
                transform.rotation = Quaternion.Slerp(fromRot, targetRot, k);
                yield return null;
            }

            if (!held && !inSocket)
            {
                SnapToAnchor();
                if (pinWhenIdle) PinIdle();
            }

            rb.isKinematic = false;
            rb.constraints = prevConstraints;
            returnCo = null;
        }

        // === Helpers ===
        Vector3 GetAnchorPos() => (anchor != null) ? anchor.position : capturedPos;
        Quaternion GetAnchorRot() => (anchor != null) ? anchor.rotation : capturedRot;

        void SnapToAnchor()
        {
            transform.SetPositionAndRotation(GetAnchorPos(), GetAnchorRot());
        }

        void PinIdle()
        {
            // Freeze all positions, keep only Yaw if you want (we'll freeze all rotations by default)
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        void Unpin()
        {
            rb.constraints = originalConstraints; // restore whatever you set in Inspector
        }

        void OnDrawGizmosSelected()
        {
            Vector3 p = (anchor != null) ? anchor.position : (Application.isPlaying ? capturedPos : transform.position);
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(p, gizmoSize);
        }
    }
}


