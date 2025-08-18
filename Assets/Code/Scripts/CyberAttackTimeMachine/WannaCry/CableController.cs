using UnityEngine;

namespace CATM_WV
{
    [RequireComponent(typeof(LineRenderer))]
    public class CableController : MonoBehaviour
    {
        public Transform endA;            // Stationary end
        public Transform endB;            // Movable end
        public float cableWidth = 0.02f;
        public int lineSegments = 20;

        private LineRenderer line;

        void Start()
        {
            // Setup LineRenderer
            line = GetComponent<LineRenderer>();
            line.positionCount = lineSegments;
            line.startWidth = cableWidth;
            line.endWidth = cableWidth;

            // Ensure ends have rigidbodies
            Rigidbody rbA = endA.GetComponent<Rigidbody>();
            if (rbA == null) rbA = endA.gameObject.AddComponent<Rigidbody>();
            rbA.isKinematic = true; // Stationary

            Rigidbody rbB = endB.GetComponent<Rigidbody>();
            if (rbB == null) rbB = endB.gameObject.AddComponent<Rigidbody>();
            rbB.isKinematic = false; // Movable
        }

        void Update()
        {
            Vector3 start = endA.position;
            Vector3 end = endB.position;

            // Update LineRenderer dynamically
            for (int i = 0; i < lineSegments; i++)
            {
                float t = i / (float)(lineSegments - 1);
                Vector3 pos = Vector3.Lerp(start, end, t);
                line.SetPosition(i, pos);
            }
        }
    }
}
