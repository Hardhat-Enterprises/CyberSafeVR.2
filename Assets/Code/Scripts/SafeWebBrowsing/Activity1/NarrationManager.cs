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
        public GameObject interactButton;

        [Header("Scene Narration Lines")]
        [TextArea(2, 5)]
        public string[] sceneMessages;
        public AudioClip[] sceneAudioClips; // One audio per message
        public int currentIndex = 0;

        [Header("Audio Clips")]
        public AudioClip textAppearClip; // Optional fallback

        private AudioSource audioSource;

        public event System.Action OnNarrationComplete;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
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
            if (audioSource.isPlaying){
                audioSource.Stop();
            }
            if (currentIndex < sceneMessages.Length)
            {
                interactButton.SetActive(false);
                // Show narration UI
                narration.SetActive(true);
                narrationText.text = sceneMessages[currentIndex];

                // Play corresponding clip if available
                if (sceneAudioClips != null && currentIndex < sceneAudioClips.Length && sceneAudioClips[currentIndex] != null)
                {
                    audioSource.PlayOneShot(sceneAudioClips[currentIndex]);
                }
                else
                {
                    PlaySound(textAppearClip);
                }

                currentIndex++;
            }
            else
            {
                // All messages played, hide narration UI and reset
                narration.SetActive(false);
                interactButton.SetActive(true);
                currentIndex = 0;

                OnNarrationComplete?.Invoke();
            }
        }

        public void ClearMessages(){
            currentIndex = 0;
            sceneMessages = new string[0];
            sceneAudioClips = new AudioClip[0];
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
