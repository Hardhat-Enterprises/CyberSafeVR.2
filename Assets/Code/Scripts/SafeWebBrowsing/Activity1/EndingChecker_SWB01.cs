using UnityEngine;

using System.Linq; // Needed for FirstOrDefault()

public class EndingChecker_SWB01 : MonoBehaviour
{
    public string correctTag = "Safe";
    public GameManager_SWB01 gameManager;
    public GameObject spotLight;

    public void CheckCurrentObject()
    {
        var socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        if (socket == null || gameManager == null)
            return;

        // Get the first object currently in the socket
        var interactable = socket.interactablesSelected.FirstOrDefault();
        if (interactable == null)
            return;

        GameObject obj = interactable.transform.gameObject;
        Debug.Log(correctTag);
        Debug.Log(obj.CompareTag(correctTag));
        if (!obj.CompareTag(correctTag))
        {
            if (spotLight != null)
                spotLight.SetActive(true);

            gameManager.SetEnding(1);
        }
    }
}
