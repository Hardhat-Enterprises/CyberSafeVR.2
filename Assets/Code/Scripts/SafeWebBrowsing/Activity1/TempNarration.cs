using UnityEngine;

namespace SWB01
{
    public class TemporaryNarration : MonoBehaviour
    {
        [Header("Temporary Narration")]
        [TextArea(2, 5)]
        public string[] tempMessages;
        public AudioClip[] tempAudioClips;

        private NarrationManager narrationManager;

        // Storage for existing narration
        private string[] savedMessages;
        private AudioClip[] savedAudioClips;

        void Start()
        {
            // Use the new recommended method to find the first active NarrationManager
            narrationManager = Object.FindFirstObjectByType<NarrationManager>();
            if (narrationManager == null)
            {
                Debug.LogError("NarrationManager not found in the scene.");
            }
        }

        public void PlayTemporaryNarration()
        {
            if (narrationManager == null || tempMessages.Length == 0) return;

            // Save current messages and audio
            savedMessages = narrationManager.sceneMessages;
            savedAudioClips = narrationManager.sceneAudioClips;

            // Replace with temporary messages and audio
            narrationManager.sceneMessages = tempMessages;
            narrationManager.sceneAudioClips = tempAudioClips;

            // Subscribe to completion event
            narrationManager.OnNarrationComplete += OnTemporaryNarrationComplete;

            // Start playing the first temporary message
            narrationManager.NextMessage();
        }

        private void OnTemporaryNarrationComplete()
        {
            Debug.Log("temp narration done");
            if (narrationManager == null) return;

            // Restore original messages and audio
            narrationManager.sceneMessages = savedMessages;
            narrationManager.sceneAudioClips = savedAudioClips;

            // Unsubscribe from the event
            narrationManager.OnNarrationComplete -= OnTemporaryNarrationComplete;

        }
    }
}
