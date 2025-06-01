using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AnswerValidator : MonoBehaviour
{
    [System.Serializable]
    public class SocketAnswerPair
    {
        public XRSocketInteractor socket;
        public string expectedTag; // Tag Answers as "A1", "A2", etc.
    }

    public SocketAnswerPair[] answerPairs;

    public GameObject winCanvas; //GoodMath
    public GameObject loseCanvas; //BadMath

    [Header("Canvas Spawn Settings")]
    public Transform cameraTransform;          // Follow XR Camera
    public float distanceFromCamera = 1.5f;    // Distance to spawn canvas in front of camera

    public void ValidateAnswers()
    {
        bool isAllCorrect = true;

        foreach (var pair in answerPairs)
        {
            var interactable = pair.socket.GetOldestInteractableSelected();

            if (interactable == null)
            {
                Debug.Log($"{pair.socket.name} is empty.");
                isAllCorrect = false;
                continue;
            }

            string actualTag = interactable.transform.tag;

            if (actualTag != pair.expectedTag)
            {
                Debug.Log($"{pair.socket.name} has WRONG object. Expected: {pair.expectedTag}, Found: {actualTag}");
                isAllCorrect = false;
            }
            else
            {
                Debug.Log($"{pair.socket.name} is correct.");
            }
        }

        if (isAllCorrect)
        {
            Debug.Log("YOU WIN! Good ending reached. Sweet lord jesus hallelujah.");
            ShowCanvasInFront(winCanvas);
            if (loseCanvas != null) loseCanvas.SetActive(false);

            GameManager.Instance.MissionCleared();
        }
        else
        {
            Debug.Log("You bad at math :'( Try again");
            ShowCanvasInFront(loseCanvas);
            if (winCanvas != null) winCanvas.SetActive(false);
        }
    }

    // Handle spawning & following in front of camera
    private void ShowCanvasInFront(GameObject canvas)
    {
        if (canvas == null || cameraTransform == null)
        {
            Debug.LogError("Canvas or Camera Transform not assigned!");
            return;
        }

        canvas.transform.SetParent(cameraTransform); // Make it follow the headset
        canvas.transform.localPosition = new Vector3(0, 0, distanceFromCamera); // Fixed offset forward
        canvas.transform.localRotation = Quaternion.identity;
        canvas.SetActive(true);
    }
}
