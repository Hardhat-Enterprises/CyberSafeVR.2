using System.Collections;
using UnityEngine;

namespace Smishing01
{
    /// <summary>
    /// Procedural audio feedback. No AudioClip assets required.
    /// Assign real clips in the Inspector to override any of the tones.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class ScenarioAudioController : MonoBehaviour
    {
        [Header("Optional clip overrides")]
        public AudioClip correctSfx;
        public AudioClip incorrectSfx;
        public AudioClip messageSfx;
        public AudioClip clickSfx;
        public AudioClip ambientClip;

        private AudioSource _sfx;
        private AudioSource _ambient;

        private void Awake()
        {
            var sources = GetComponents<AudioSource>();
            _sfx     = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
            _ambient = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

            _sfx.playOnAwake = false;
            _ambient.loop    = true;
            _ambient.volume  = 0.12f;

            if (ambientClip != null)
            {
                _ambient.clip = ambientClip;
                _ambient.Play();
            }
        }

        public void PlayCorrect()   => PlayOrChord(correctSfx,   new[] { 523.25f, 659.25f, 783.99f }, 0.35f); // C5-E5-G5
        public void PlayIncorrect() => PlayOrTone (incorrectSfx, 196f, 0.4f);                                  // G3
        public void PlayMessage()   => PlayOrChord(messageSfx,   new[] { 587.33f, 880f },            0.20f); // D5-A5
        public void PlayClick()     => PlayOrTone (clickSfx,     1046.5f, 0.05f);                              // C6 blip

        // ── Private ───────────────────────────────────────────────────────────

        private void PlayOrTone(AudioClip clip, float toneHz, float duration)
        {
            if (clip != null) { _sfx.PlayOneShot(clip); return; }
            _sfx.PlayOneShot(BuildTone(new[] { toneHz }, duration));
        }

        private void PlayOrChord(AudioClip clip, float[] freqs, float duration)
        {
            if (clip != null) { _sfx.PlayOneShot(clip); return; }
            _sfx.PlayOneShot(BuildTone(freqs, duration));
        }

        private static AudioClip BuildTone(float[] freqs, float duration)
        {
            int sampleRate = AudioSettings.outputSampleRate;
            int samples    = Mathf.CeilToInt(sampleRate * duration);
            float[] data   = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t   = (float)i / sampleRate;
                float env = Mathf.Clamp01(1f - t / duration);
                float s   = 0f;
                foreach (var f in freqs) s += Mathf.Sin(2f * Mathf.PI * f * t);
                data[i] = (s / freqs.Length) * env * 0.4f;
            }

            var clip = AudioClip.Create("tone", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
