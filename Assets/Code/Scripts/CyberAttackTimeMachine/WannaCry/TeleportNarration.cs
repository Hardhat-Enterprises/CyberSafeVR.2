using UnityEngine;
using TMPro;

namespace CATM_WC
{
    public class TeleportNarration : MonoBehaviour
    {
        [Header("UI Elements")]
        public GuideNarrationManager narrationManager;
        
        [Header("Condition")]
        public int cond = 0; // 0 = not fixed, 1 = fixed

        [Header("Not Fixed Lines")]
        [TextArea(2, 5)] public string[] nfixedMessages;
        public AudioClip[] nfixedAudioClips;

        [Header("Fixed Lines")]
        [TextArea(2, 5)] public string[] fixedMessages;
        public AudioClip[] fixedAudioClips;

        public void SetCond() => cond = 1;

        public void ChooseMessages()
        {
            if (cond == 0) SendMessages(nfixedMessages, nfixedAudioClips);
            else SendMessages(fixedMessages, fixedAudioClips);
        }

        private void SendMessages(string[] sceneMessages, AudioClip[] sceneAudioClips)
        {
            narrationManager.ClearMessages();
            for(int i = 0; i < sceneMessages.Length; i++){
                narrationManager.AddMessage(sceneMessages[i]);
                if(i < sceneAudioClips.Length) narrationManager.AddAudioClip(sceneAudioClips[i]);
            }
            narrationManager.NextMessage();
        }
    }
}
