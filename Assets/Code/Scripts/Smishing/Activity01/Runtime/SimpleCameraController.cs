using UnityEngine;

namespace Smishing01
{
    /// <summary>
    /// Desktop WASD + right-mouse-look camera for testing without a VR headset.
    /// Automatically disables itself if an XR device is active.
    /// </summary>
    public class SimpleCameraController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float lookSpeed = 2f;

        private float _yaw, _pitch;
        private bool  _active;

        private void Start()
        {
#if UNITY_2022_3_OR_NEWER
            if (UnityEngine.XR.XRSettings.isDeviceActive)
            {
                enabled = false;
                return;
            }
#endif
            _yaw   = transform.eulerAngles.y;
            _pitch = transform.eulerAngles.x;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))  { Cursor.lockState = CursorLockMode.Locked; _active = true; }
            if (Input.GetKeyDown(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; _active = false; }

            if (_active)
            {
                _yaw   += Input.GetAxis("Mouse X") * lookSpeed;
                _pitch -= Input.GetAxis("Mouse Y") * lookSpeed;
                _pitch  = Mathf.Clamp(_pitch, -80f, 80f);
                transform.eulerAngles = new Vector3(_pitch, _yaw, 0f);
            }

            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir += transform.forward;
            if (Input.GetKey(KeyCode.S)) dir -= transform.forward;
            if (Input.GetKey(KeyCode.A)) dir -= transform.right;
            if (Input.GetKey(KeyCode.D)) dir += transform.right;
            transform.position += dir.normalized * (moveSpeed * Time.deltaTime);
        }
    }
}
