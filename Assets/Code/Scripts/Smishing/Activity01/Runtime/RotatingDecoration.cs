using UnityEngine;

namespace Smishing01
{
    /// <summary>
    /// Slowly rotates a transform around a chosen axis. Used for ambient
    /// wall decorations and the desktop monitor screen.
    /// </summary>
    public class RotatingDecoration : MonoBehaviour
    {
        [SerializeField] private Vector3 axis      = Vector3.up;
        [SerializeField] private float   speedDeg  = 8f;

        private void Update()
        {
            transform.Rotate(axis.normalized, speedDeg * Time.deltaTime, Space.Self);
        }
    }
}
