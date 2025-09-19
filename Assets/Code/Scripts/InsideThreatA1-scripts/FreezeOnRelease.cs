using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


namespace InsiderThreat01
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class FreezeOnRelease : MonoBehaviour
    {
        private Rigidbody rb;
        private XRGrabInteractable grab;

        // While released: don't let it slide or tip
        private static readonly RigidbodyConstraints Released =
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        // While held: allow moving; keep it flat
        private static readonly RigidbodyConstraints Held =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            grab = GetComponent<XRGrabInteractable>();
        }

        void OnEnable()
        {
            grab.selectEntered.AddListener(OnGrabbed);
            grab.selectExited.AddListener(OnReleased);
        }

        void OnDisable()
        {
            grab.selectEntered.RemoveListener(OnGrabbed);
            grab.selectExited.RemoveListener(OnReleased);
        }

        void Start()
        {
            rb.constraints = Released; // start locked on desk
        }

        void OnGrabbed(SelectEnterEventArgs _)
        {
            rb.constraints = Held;     // free X/Z while holding
            rb.isKinematic = false;    // make sure it's throwable if you like
        }

        void OnReleased(SelectExitEventArgs _)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = Released; // lock again so it won't creep
            rb.Sleep();
        }
    }
}


