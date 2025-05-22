using UnityEngine;

public class PaperPinTrigger_SSA01 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paper"))
        {
            string userName = GetUserNameFromPaper(other.gameObject);

            AccessListController_SSA1 controller = FindFirstObjectByType<AccessListController_SSA1>();

            if (controller != null)
            {
                controller.AddUser(userName);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Paper"))
        {
            string userName = GetUserNameFromPaper(other.gameObject);

            AccessListController_SSA1 controller = FindFirstObjectByType<AccessListController_SSA1>();

            if (controller != null)
            {
                controller.RemoveUser(userName);
            }
        }
    }

    private string GetUserNameFromPaper(GameObject paper)
    {
        PaperLabel_SSA01 label = paper.GetComponent<PaperLabel_SSA01>();
        return label != null ? label.userName : "Unknown";
    }
}
