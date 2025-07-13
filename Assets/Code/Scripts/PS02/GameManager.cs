using UnityEngine;

namespace PS02
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GameObject breachCanvas;
        public GameObject breachLight;         // Spotlight GameObject
        public PopupSpawner popupSpawner;      // Drag the spawner here

        [Header("Canvas Spawn Settings")]
        public Transform cameraTransform;
        public float distanceFromCamera = 1.5f;

        [Header("Mission Music")]
        public AudioSource missionMusicSource; // Mission music (time ticking sound)

        void Awake()
        {
            Instance = this;
            if (breachCanvas != null)
                breachCanvas.SetActive(false);
        }

        public void TriggerBreach()
        {
            Debug.Log("Breach triggered!");

            ShowCanvasInFront(breachCanvas);

            if (breachLight != null)
                breachLight.SetActive(true);

            if (popupSpawner != null)
            {
                popupSpawner.StopSpawning(); // THIS STOPS POPUPS
                Debug.Log("Called StopSpawning from GameManager");
            }
        }

        public void MissionCleared()
        {
            Debug.Log("? Mission cleared! Stopping popups.");

            if (popupSpawner != null)
            {
                popupSpawner.StopSpawning();
            }
        }

        // Canvas positioning logic (spawn in front of camera)
        private void ShowCanvasInFront(GameObject canvas)
        {
            if (canvas == null || cameraTransform == null)
            {
                Debug.LogError("Canvas or Camera Transform not assigned!");
                return;
            }

            canvas.transform.SetParent(cameraTransform); // Follow the headset
            canvas.transform.localPosition = new Vector3(0, 0, distanceFromCamera); // Offset in front
            canvas.transform.localRotation = Quaternion.identity;
            canvas.SetActive(true);
        }
    }
}