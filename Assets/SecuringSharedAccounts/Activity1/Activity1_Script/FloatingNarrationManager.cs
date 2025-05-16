using UnityEngine;
using TMPro;

public class FloatingNarrationManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject narrationCanvas;
    public GameObject narration;
    public TextMeshProUGUI narrationText;

    [Header("Scene Narration Lines")]
    [TextArea(2, 5)]
    public string[] sceneMessages;
    private int currentIndex = 0;

    void Start()
    {
        if (sceneMessages.Length > 0)
        {
            NextMessage();
        }
    }

    public void AddMessage(string newMessage)
    {
        string[] updatedMessages = new string[sceneMessages.Length + 1];
        for (int i = 0; i < sceneMessages.Length; i++)
        {
            updatedMessages[i] = sceneMessages[i];
        }
        updatedMessages[sceneMessages.Length] = newMessage;
        sceneMessages = updatedMessages;
    }


    // Called by a "Next" button
    public void NextMessage()
    {
        narration.SetActive(true);
        if (currentIndex < sceneMessages.Length)
        {
            narrationText.text = sceneMessages[currentIndex];
            currentIndex++;
        }
        else
        {
            narration.SetActive(false);
            currentIndex = 0; // Reset for reuse if needed
            sceneMessages = new string[0]; // Clear the scene messages

        }
        
    }
}
