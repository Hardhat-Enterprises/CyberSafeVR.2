using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

namespace CATM_WC
{
    public class TagSocketValidator : MonoBehaviour
    {
        public GameManager GameManager;
        public string expectedImpactText;
        public AudioClip correctSound;
        public AudioClip incorrectSound;
        public float lockDelay = 0.3f; // time in seconds before locking correct card

        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;
        private AudioSource audioSource;

        void Awake()
        {
            socketInteractor = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
            audioSource = GetComponent<AudioSource>();

            socketInteractor.selectEntered.AddListener(ValidateMatch);
        }

        private void ValidateMatch(SelectEnterEventArgs args)
        {
            GameObject card = args.interactableObject.transform.gameObject;

            if (!card.CompareTag("Cards"))
            {
                PlaySound(incorrectSound);
                return;
            }

            CardLabel label = card.GetComponent<CardLabel>();
            if (label != null && label.impactText == expectedImpactText)
            {
                PlaySound(correctSound);
                StartCoroutine(LockCardAfterDelay(card, lockDelay));
                GameManager.CheckCardForDoor();
            }
            else
            {
                PlaySound(incorrectSound);
                // Wrong card stays in socket, no rejection
            }
        }

        private IEnumerator LockCardAfterDelay(GameObject card, float delay)
        {
            yield return new WaitForSeconds(delay);

            var grab = card.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab != null)
            {
                grab.enabled = false; // Prevent further grabbing
            }

            var rb = card.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        private void PlaySound(AudioClip sound)
        {
            if (audioSource != null && sound != null)
            {
                audioSource.PlayOneShot(sound);
            }
        }
    }
}
