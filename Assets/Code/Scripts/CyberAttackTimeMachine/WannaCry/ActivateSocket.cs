using UnityEngine;


namespace CATM_WC
{
    public class ActivateSocket : MonoBehaviour
    {
        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;

        void Awake()
        {
            socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
            if (socket == null)
                Debug.LogWarning("No XRSocketInteractor found on " + gameObject.name);
        }

        // Call this to toggle the socket interactor
        public void ToggleSocket()
        {
            if (socket != null)
            {
                socket.enabled = !socket.enabled;
                }
        }
    }
}
