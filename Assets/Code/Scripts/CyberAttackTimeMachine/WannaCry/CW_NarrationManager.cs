using UnityEngine;
using TMPro;

namespace CATM_WC
{
    public class CW_NarrationManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject narrationCanvas;
        public GameObject narration;
        public TextMeshProUGUI narrationText;
        public GameObject narrationTrigger;
        
        [Header("NPC Reference")]
        public GuideNPC guideNPC;

        [Header("Audio Clips")]
        public AudioClip textAppearClip;

        [Header("Timeline and Condition")]
        public int time = 1; // 0 = past, 1 = future
        public int cond = 0; // 0 = disaster, 1 = fixed

        [Header("Disaster Narration Lines")]
        [TextArea(2, 5)] public string[] disasterMessages;
        public AudioClip[] disasterAudioClips;

        [Header("Saved Narration Lines")]
        [TextArea(2, 5)] public string[] savedMessages;
        public AudioClip[] savedAudioClips;

        [Header("Past Narration Lines")]
        [TextArea(2, 5)] public string[] pastMessages;
        public AudioClip[] pastAudioClips;

        private int currentIndex = 0;

        [Header("Post-Narration")]
        public bool activateObjectAfterNarration = true;
        public GameObject objectToActivateAfterNarration;

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

            // NarrationTrigger is a sibling of narrationCanvas
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

            // GuideNPC is a component on the grandparent
            if (narrationCanvasParent != null)
            {
                guideNPC = narrationCanvasParent.GetComponent<GuideNPC>();
            }
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (guideNPC != null)
            {
                guideNPC.onFacedPlayer.AddListener(OnNPCFacedPlayer);
            }
        }

        public void SetTime(int i) => time = i;
        public void SetCond() => cond = 1;

        public void NextMessage()
        {
            narrationTrigger.SetActive(false);
            narration.SetActive(false);
            guideNPC.FacePlayer();
        }

        private void OnNPCFacedPlayer()
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            narration.SetActive(true);

            if (time == 0)
            {
                ShowMessage(pastMessages, pastAudioClips);
            }
            else
            {
                if (cond == 0)
                    ShowMessage(disasterMessages, disasterAudioClips);
                else
                    ShowMessage(savedMessages, savedAudioClips);
            }
        }

        private void ShowMessage(string[] sceneMessages, AudioClip[] sceneAudioClips)
        {
            if (currentIndex < sceneMessages.Length)
            {
                narrationText.text = sceneMessages[currentIndex];

                if (sceneAudioClips != null &&
                    currentIndex < sceneAudioClips.Length &&
                    sceneAudioClips[currentIndex] != null)
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
                narration.SetActive(false);
                currentIndex = 0;

                narrationTrigger?.SetActive(true);

                if (activateObjectAfterNarration && objectToActivateAfterNarration != null)
                {
                    objectToActivateAfterNarration.SetActive(true);
                    activateObjectAfterNarration = false;
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
