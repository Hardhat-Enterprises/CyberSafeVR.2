using System;
using System.Collections.Generic;
using UnityEngine;

namespace InsiderThreat01 {
    public enum SfxEvent { Select, Correct, Wrong, Hint, Confirm, Cancel }

    [Serializable]
    public class SfxEntry
    {
        public SfxEvent key;
        public AudioClip[] clips;          // allow multiple variations
        [Range(0f, 1f)] public float volume = 1f;
        public Vector2 pitchJitter = new Vector2(1f, 1f); // e.g., (0.95, 1.05)
        public float cooldown = 0f;        // avoid rapid spam if needed
        [HideInInspector] public float nextTime = 0f;
    }

    public class SFXManager : MonoBehaviour
    {
        public static SFXManager I { get; private set; }
        [SerializeField] AudioSource src;        // 2D source for UI/quiz
        [SerializeField] List<SfxEntry> entries = new();

        Dictionary<SfxEvent, SfxEntry> map;

        void Awake()
        {
            if (I != null && I != this) { Destroy(gameObject); return; }
            I = this;
            DontDestroyOnLoad(gameObject);

            if (!src) src = gameObject.AddComponent<AudioSource>();
            src.spatialBlend = 0f; // 2D UI audio

            map = new Dictionary<SfxEvent, SfxEntry>(entries.Count);
            foreach (var e in entries) map[e.key] = e;
        }

        public void Play(SfxEvent key)
        {
            if (!map.TryGetValue(key, out var e) || e.clips == null || e.clips.Length == 0) return;
            if (Time.unscaledTime < e.nextTime) return; // cooldown
            var clip = e.clips[UnityEngine.Random.Range(0, e.clips.Length)];
            src.pitch = UnityEngine.Random.Range(e.pitchJitter.x, e.pitchJitter.y);
            src.PlayOneShot(clip, e.volume);
            e.nextTime = Time.unscaledTime + e.cooldown;
        }

        // Overload for world 3D sounds if you need them:
        public void PlayAt(SfxEvent key, Vector3 pos)
        {
            if (!map.TryGetValue(key, out var e) || e.clips.Length == 0) return;
            var clip = e.clips[UnityEngine.Random.Range(0, e.clips.Length)];
            AudioSource.PlayClipAtPoint(clip, pos, e.volume);
        }
    }
}

