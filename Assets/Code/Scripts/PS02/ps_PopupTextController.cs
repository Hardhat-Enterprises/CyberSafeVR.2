using UnityEngine;

public class PopupTextController : MonoBehaviour
{

    public GameObject popupRoot;

    public void OnYesButtonClicked()
    {
        Debug.Log("Yes button was pressed");

        if (popupRoot != null)
        {
            popupRoot.SetActive(false); // Close the popup
        }

        GameManager.Instance.TriggerBreach(); // Call breach logic
    }

    public void HandleNo()
    {
        Debug.Log("No button pressed. Destroying popup.");
        Destroy(gameObject);
    }


}
