using UnityEngine;
using TMPro;

namespace CATM_WC
{
    public class GuideNarrationManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject narrationCanvas;
        public GameObject narration;
        public TextMeshProUGUI narrationText;
        public GameObject narrationTrigger;

        [Header("Scene Narration Lines")]
        [TextArea(2, 5)]
        public string[] sceneMessages;
        public AudioClip[] sceneAudioClips; // One audio per message
        private int currentIndex = 0;

        [Header("Audio Clips")]
        public AudioClip textAppearClip; // Optional fallback

        private AudioSource audioSource;

        void Awake()
        {
            // Parent is the NarrationCanvas
            narrationCanvas = transform.parent.gameObject;

            // Narration is a sibling under the same parent
            foreach (Transform child in narrationCanvas.transform)
            {
                if (child != transform)
                {
                    // Just grab the first sibling that's not this script's GameObject
                    narration = child.gameObject;
                    break;
                }
            }

            // Narration text is somewhere inside narration
            if (narration != null)
            {
                narrationText = narration.GetComponentInChildren<TextMeshProUGUI>(true);
            }

            // narrationTrigger is a sibling of narrationCanvas
            Transform narrationCanvasParent = narrationCanvas.transform.parent;
            if (narrationCanvasParent != null)
            {
                foreach (Transform sibling in narrationCanvasParent)
                {
                    if (sibling != narrationCanvas.transform)
                    {
                        narrationTrigger = sibling.gameObject;
                        break;
                    }
                }
            }
        }

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
                narrationTrigger.SetActive(false);
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
                narrationTrigger.SetActive(true);
                currentIndex = 0;
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
