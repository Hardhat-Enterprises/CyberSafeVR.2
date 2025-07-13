using UnityEngine;
using UnityEngine.SceneManagement;

namespace PS02
{
    public class OTPPopupManager : MonoBehaviour
    {
        public GameObject breachCanvas;
        public Transform cameraTransform;

        [Header("Canvas Settings")]
        public float distanceFromCamera = 1.5f; // Distance to spawn the canvas in front of the camera

        public void ShowBreachCanvas()
        {
            if (breachCanvas == null)
            {
                Debug.LogError("Breach Canvas not assigned!");
                return;
            }

            if (cameraTransform == null)
            {
                Debug.LogError("Camera Transform not assigned!");
                return;
            }

            // Set the canvas as a child of the camera so that it follows the camera's position and rotation
            breachCanvas.transform.SetParent(cameraTransform);

            // Position the canvas in front of the camera (as a local offset)
            breachCanvas.transform.localPosition = new Vector3(0, 0, distanceFromCamera);
            breachCanvas.transform.localRotation = Quaternion.identity;

            breachCanvas.SetActive(true);
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}