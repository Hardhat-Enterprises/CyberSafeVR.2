using UnityEngine;
using UnityEngine.UI; 

namespace SWB01
{
    public class ButtonPressChecker : MonoBehaviour
    {
        public Button yourButton;  // Assign the button in the Inspector

        void Start()
        {
            // Add an event listener to the button's onClick event
            yourButton.onClick.AddListener(OnButtonClick);
        }

        void OnButtonClick()
        {
            // Log when the button is clicked
            Debug.Log("Button has been clicked!");
        }
    }
}
