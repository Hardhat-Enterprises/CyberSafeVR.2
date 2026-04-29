using UnityEngine;

namespace Smishing01
{
    /// <summary>
    /// Adds a subtle floating + tilting animation to a transform, used for
    /// the phone canvas when it's on screen. Brings the scene to life.
    /// </summary>
    public class PhoneIdleAnimation : MonoBehaviour
    {
        [SerializeField] private float bobAmplitude = 0.012f;
        [SerializeField] private float bobFrequency = 0.6f;
        [SerializeField] private float tiltAmount   = 1.2f;
        [SerializeField] private float tiltFrequency= 0.4f;

        private Vector3 _basePos;
        private Quaternion _baseRot;

        private void OnEnable()
        {
            _basePos = transform.localPosition;
            _baseRot = transform.localRotation;
        }

        private void Update()
        {
            float bob = Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f) * bobAmplitude;
            float tlt = Mathf.Sin(Time.time * tiltFrequency * Mathf.PI * 2f) * tiltAmount;

            transform.localPosition = _basePos + new Vector3(0f, bob, 0f);
            transform.localRotation = _baseRot * Quaternion.Euler(tlt * 0.3f, tlt, 0f);
        }
    }
}
