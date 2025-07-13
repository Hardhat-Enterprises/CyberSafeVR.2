using UnityEngine;

namespace IT01
{
    public class MoveCamera : MonoBehaviour
    {
        public Transform cameraPosition;

        private void Update()
        {
            transform.position = cameraPosition.position;
        }
    }
}
