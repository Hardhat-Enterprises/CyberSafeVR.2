using UnityEngine;

public class DeactivateInfoOnTrigger_SWB01 : MonoBehaviour
{
    public string targetName = "Info";
    public NarrationManager_SWB01 narrationManager;

    private void OnTriggerEnter(Collider other)
    {
        // Find all GameObjects, including inactive ones
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(
            FindObjectsSortMode.None
        );

        foreach (GameObject obj in allObjects)
        {
            if (obj.name == targetName)
            {
                obj.SetActive(false);
            }
        }
        narrationManager.NextMessage();
        // Deactivate this trigger object after doing its job
        gameObject.SetActive(false);
    }
}
