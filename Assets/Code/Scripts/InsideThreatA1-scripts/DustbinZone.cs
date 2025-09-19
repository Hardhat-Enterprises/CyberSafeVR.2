using UnityEngine;

namespace InsiderThreat01 {
    public class DustbinZone : MonoBehaviour
    {
        [Header("Snap Target (inside the bin)")]
        public Transform snapPoint;

        [Header("Fire VFX (Particle System)")]
        public ParticleSystem fireEffect;

        void Awake()
        {
            // Auto-find by name if not wired in Inspector
            if (snapPoint == null)
            {
                var t = transform.parent != null ? transform.parent : transform;
                var found = t.Find("SnapPoint");
                if (found != null) snapPoint = found;
            }
            if (fireEffect == null)
            {
                var t = transform.parent != null ? transform.parent : transform;
                var found = t.Find("FireFX");
                if (found) fireEffect = found.GetComponent<ParticleSystem>();
            }
        }

        void Reset()
        {
            var col = GetComponent<Collider>();
            if (col) col.isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            var note = other.GetComponentInParent<StickyNote>();
            if (note == null || snapPoint == null) return;

            note.SnapIntoBin(snapPoint, fireEffect);
        }
    }
}


