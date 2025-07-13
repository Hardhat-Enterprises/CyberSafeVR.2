using UnityEngine;
using TMPro;


namespace SWB01
{
    public class NarrationManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject narrationCanvas;
        public GameObject narration;
        public TextMeshProUGUI narrationText;

        [Header("Scene Narration Lines")]
        [TextArea(2, 5)]
        public string[] sceneMessages;
        public AudioClip[] sceneAudioClips; // New: One audio per message
        private int currentIndex = 0;

        [Header("NPC Reference")]
        public GuideNPC guideNPC; // Assign in Inspector

        [Header("Audio Clips")]
        public AudioClip textAppearClip; // Optional fallback

        private AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            if (sceneMessages.Length > 0 && guideNPC != null)
            {
                guideNPC.onReachedPlayer.AddListener(OnNPCReachedPlayer);
                NextMessage(); // Optional: start first message immediately
            }
        }

        public void AddMessage(string newMessage)
        {
            string[] updatedMessages = new string[sceneMessages.Length + 1];
            for (int i = 0; i < sceneMessages.Length; i++)
            {
                updatedMessages[i] = sceneMessages[i];
            }
            updatedMessages[sceneMessages.Length] = newMessage;
            sceneMessages = updatedMessages;
        }

        public void AddAudioClip(AudioClip newClip)
        {
            AudioClip[] updatedClips = new AudioClip[sceneAudioClips.Length + 1];
            for (int i = 0; i < sceneAudioClips.Length; i++)
            {
                updatedClips[i] = sceneAudioClips[i];
            }
            updatedClips[sceneAudioClips.Length] = newClip;
            sceneAudioClips = updatedClips;
        }

        public void NextMessage()
        {
            guideNPC.GoToPlayer();
            narration.SetActive(false);
        }

        private void OnNPCReachedPlayer()
        {
            // Stop any current sound
            if (audioSource.isPlaying)
                audioSource.Stop();
            narration.SetActive(true);

            if (currentIndex < sceneMessages.Length)
            {
                narrationText.text = sceneMessages[currentIndex];
                // Play corresponding clip if available
                if (sceneAudioClips != null && currentIndex < sceneAudioClips.Length && sceneAudioClips[currentIndex] != null)
                {
                    audioSource.PlayOneShot(sceneAudioClips[currentIndex]);
                }
                else
                {
                    // Fallback to textAppearClip if no matching audio
                    PlaySound(textAppearClip);
                }

                currentIndex++;
            }
            else
            {
                narration.SetActive(false);
                currentIndex = 0;
                sceneMessages = new string[0];
                sceneAudioClips = new AudioClip[0];

                if (guideNPC != null)
                {
                    guideNPC.WalkAway();
                }
            }
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}