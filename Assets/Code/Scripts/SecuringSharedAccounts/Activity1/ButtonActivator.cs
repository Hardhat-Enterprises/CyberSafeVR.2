using UnityEngine;
using UnityEngine.UI;  // For UI button reference

public class ButtonActivator_SSA01 : MonoBehaviour
{
    [Header("Buttons")]
    public Button button1;  // First button
    public Button button2;  // Second button
    public Button activatingButton;  // Button that becomes active once the others are clicked

    private bool button1Clicked = false;
    private bool button2Clicked = false;

    void Start()
    {
        // Add listeners to the buttons to detect when they are clicked
        button1.onClick.AddListener(OnButton1Clicked);
        button2.onClick.AddListener(OnButton2Clicked);
    }

    // When button 1 is clicked
    void OnButton1Clicked()
    {
        button1Clicked = true;
        CheckButtonsStatus();
    }

    // When button 2 is clicked
    void OnButton2Clicked()
    {
        button2Clicked = true;
        CheckButtonsStatus();
    }

    // Checks if both button1 and button2 have been clicked
    void CheckButtonsStatus()
    {
        if (button1Clicked && button2Clicked)
        {
            // Enable button3
            activatingButton.interactable = true;
        }
    }
}
