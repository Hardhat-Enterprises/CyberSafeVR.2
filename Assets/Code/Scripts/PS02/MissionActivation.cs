using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace PS02
{
    public class PlayAudioThenActivate : MonoBehaviour
    {
        public AudioSource audioSource;
        public GameObject[] objectsToActivate;

        private XRGrabInteractable grabInteractable;
        private bool hasBeenGrabbed = false; // Track if it has been grabbed

        [Header("Mission Settings")]
        public bool triggerBreachOnActivate = false;


        void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnDropped);
        }

        void OnDestroy()
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnDropped);
        }

        void OnGrabbed(SelectEnterEventArgs args)
        {
            hasBeenGrabbed = true;
        }

        void OnDropped(SelectExitEventArgs args)
        {
            if (hasBeenGrabbed && audioSource != null && !audioSource.isPlaying)
            {
                Debug.Log("Playing audio after drop...");
                audioSource.Play();
                Invoke(nameof(ActivateObjects), audioSource.clip.length);
                hasBeenGrabbed = false;
            }
        }

        void ActivateObjects()
        {
            foreach (GameObject obj in objectsToActivate)
            {
                obj.SetActive(true);
            }
            Debug.Log("Objects activated after audio finished.");

            if (triggerBreachOnActivate)
            {
                GameManager.Instance.TriggerBreach();
            }
        }

    }
}