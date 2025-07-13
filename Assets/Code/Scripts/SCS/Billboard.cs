using UnityEngine;

namespace SCS_DS
{
    public class Billboard : MonoBehaviour
    {
        private Camera mainCam;

        void Start()
        {
            mainCam = Camera.main;
        }

        void LateUpdate()
        {
            if (mainCam != null)
                transform.forward = mainCam.transform.forward;
        }
    }
}