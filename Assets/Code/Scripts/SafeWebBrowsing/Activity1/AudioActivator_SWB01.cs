using UnityEngine;

public class DelayedAudioEnabler_SWB01 : MonoBehaviour
{
    [Tooltip("How long to wait before enabling AudioSources (in seconds)")]
    public float delay = 1.0f;

    void Start()
    {
        Invoke(nameof(EnableAllMarkedAudioSources), delay);
    }

    void EnableAllMarkedAudioSources()
    {
        DelayedAudio_SWB01[] targets = Object.FindObjectsByType<DelayedAudio_SWB01>(FindObjectsSortMode.None);

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
