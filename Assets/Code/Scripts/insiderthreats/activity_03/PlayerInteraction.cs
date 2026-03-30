using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace InsiderThreat03
{
    /// <summary>
    /// Handles player interactions in the InsiderThreat03 VR scene.
    ///
    /// BUG FIX: The original script used Input.GetKeyDown(KeyCode.E) and
    /// Camera.main — both are desktop/keyboard patterns that do NOT work in VR.
    /// Replaced with XR Interaction Toolkit event-driven approach:
    ///   - Attach an XRRayInteractor to each hand controller in the scene.
    ///   - Hook the XRBaseInteractable's "Select Entered" UnityEvent to
    ///     Interactable.OnSelect() directly in the Inspector, OR
    ///   - Use this script as a receiver on the interactor.
    ///
    /// SCENE SETUP:
    ///   1. On each workstation prefab, add an XRSimpleInteractable component.
    ///   2. In XRSimpleInteractable > Select Entered > add Interactable.OnSelect().
    ///   3. This PlayerInteraction script can then be removed — the XR Toolkit
    ///      handles wiring natively. Keep it only if you need custom logic beyond
    ///      the standard select event.
    /// </summary>
    [RequireComponent(typeof(XRBaseInteractor))]
    public class PlayerInteraction : MonoBehaviour
    {
        private XRBaseInteractor interactor;

        void Awake()
        {
            interactor = GetComponent<XRBaseInteractor>();

            if (interactor == null)
            {
                Debug.LogError("[PlayerInteraction] No XRBaseInteractor found on this " +
                    "GameObject. Attach XRRayInteractor or XRDirectInteractor.");
                return;
            }

            // BUG FIX: replaces Input.GetKeyDown() with XR Toolkit's event system.
            interactor.selectEntered.AddListener(OnSelectEntered);
        }

        void OnDestroy()
        {
            if (interactor != null)
                interactor.selectEntered.RemoveListener(OnSelectEntered);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            // Try to get an Interactable component from whatever was selected.
            var interactable = args.interactableObject as MonoBehaviour;
            if (interactable == null) return;

            var target = interactable.GetComponent<Interactable>();
            if (target != null)
            {
                Debug.Log($"[PlayerInteraction] VR selected: {interactable.name}");
                target.OnSelect();
            }
        }
    }
}