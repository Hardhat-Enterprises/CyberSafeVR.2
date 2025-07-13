using UnityEngine;

namespace SWB01
{
    public class ObjectDropSound : MonoBehaviour
    {
        private AudioSource audioSource;

        [Header("Audio Clips")]
        public AudioClip dropSound;  // Sound when the object hits the ground

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // This method will be called when the object collides with anything (like the ground)
        private void OnCollisionEnter(Collision collision)
        {
            if (dropSound != null)
            {
            audioSource.PlayOneShot(dropSound, 0.7f);
            }
        }
    }
}
