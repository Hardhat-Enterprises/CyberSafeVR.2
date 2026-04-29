using UnityEngine;

namespace Smishing01
{
    /// <summary>
    /// Pulses a Light's intensity in a soft sine wave. Used for accent
    /// lights to make the scene feel alive.
    /// </summary>
    [RequireComponent(typeof(Light))]
    public class PulseGlow : MonoBehaviour
    {
        [SerializeField] private float minIntensity = 1.5f;
        [SerializeField] private float maxIntensity = 3.0f;
        [SerializeField] private float speed        = 0.6f;

        private Light _light;
        private float _phase;

        private void Awake()
        {
            _light = GetComponent<Light>();
            _phase = Random.Range(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            float t = (Mathf.Sin(Time.time * speed * Mathf.PI * 2f + _phase) + 1f) * 0.5f;
            _light.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        }
    }
}
