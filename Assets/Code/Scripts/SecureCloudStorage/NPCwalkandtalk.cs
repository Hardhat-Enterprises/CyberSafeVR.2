using UnityEngine;
using UnityEngine.AI;
using TMPro;

namespace SCS_DS
{
    [RequireComponent(typeof(NavMeshAgent), typeof(AudioSource))]
    public class NPCWalkerAndTalk : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("The Transform (e.g. player feet) that NPC will walk towards")]
        public Transform playerTarget;
        [Tooltip("How close (in meters) the NPC stops from the target")]
        public float stopDistance = 1f;

        [Header("Dialogue UI")]
        [Tooltip("Root GameObject of the dialogue UI (contains Canvas, Text, Billboard)")]
        public GameObject dialogueUIRoot;
        [Tooltip("TextMeshProUGUI component for the bubble text")]
        public TextMeshProUGUI dialogueText;

        [Header("Voice (optional)")]
        [Tooltip("Audio clip to play when NPC reaches you")]
        public AudioClip voiceClip;

        private NavMeshAgent agent;
        private AudioSource audioSource;
        private bool hasSpoken = false;

        void Start()
        {
            agent       = GetComponent<NavMeshAgent>();
            audioSource = GetComponent<AudioSource>();

            // Hide the dialogue UI at start
            if (dialogueUIRoot != null)
                dialogueUIRoot.SetActive(false);

            // Begin walking toward player
            if (playerTarget != null)
                agent.SetDestination(playerTarget.position);

            // Ensure agent stops at the correct distance
            agent.stoppingDistance = stopDistance;
        }

        void Update()
        {
            if (hasSpoken || playerTarget == null)
                return;

            // When close enough, stop & speak
            if (!agent.pathPending && agent.remainingDistance <= stopDistance + 0.05f)
            {
                hasSpoken     = true;
                agent.isStopped = true;

                // Rotate to face the player
                transform.Rotate(0f, 90f, 0f, Space.Self);

                // Show the bubble UI
                ShowDialogue(
                    "Welcome to the Secure Cloud Storage & Data Sharing module. " +
                    "To enter the learning program, click on the cloud application on the monitor and turn your head to the right. " +
                    "An enlarged simulation screen will help you complete the learning."
                );

                // Play voice if provided
                if (voiceClip != null)
                {
                    audioSource.clip = voiceClip;
                    audioSource.Play();
                }
            }
        }

        /// <summary>
        /// Activates the UI root and sets the text.
        /// </summary>
        public void ShowDialogue(string message)
        {
            if (dialogueUIRoot != null)
                dialogueUIRoot.SetActive(true);

            if (dialogueText != null)
                dialogueText.text = message;
        }
    }
}