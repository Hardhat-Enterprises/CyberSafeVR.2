using UnityEngine;

namespace SWB01
{
    public class DelayedAudioEnabler : MonoBehaviour
    {
        [Tooltip("How long to wait before enabling AudioSources (in seconds)")]
        public float delay = 1.0f;

        void Start()
        {
            Invoke(nameof(EnableAllMarkedAudioSources), delay);
        }

        void EnableAllMarkedAudioSources()
        {
            DelayedAudio[] targets = Object.FindObjectsByType<DelayedAudio>(FindObjectsSortMode.None);

            foreach (var delayed in targets)
            {
                AudioSource[] sources = delayed.GetComponentsInChildren<AudioSource>(true);
                foreach (AudioSource source in sources)
                {
                    source.enabled = true;
                }
            }
        }
    }
}
