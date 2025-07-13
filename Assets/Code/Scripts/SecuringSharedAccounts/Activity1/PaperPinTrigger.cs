using UnityEngine;

namespace SSA01
{
    public class PaperPinTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Paper"))
            {
                string userName = GetUserNameFromPaper(other.gameObject);

                AccessListController controller = FindFirstObjectByType<AccessListController>();

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

                AccessListController controller = FindFirstObjectByType<AccessListController>();

                if (controller != null)
                {
                    controller.RemoveUser(userName);
                }
            }
        }

        private string GetUserNameFromPaper(GameObject paper)
        {
            PaperLabel label = paper.GetComponent<PaperLabel>();
            return label != null ? label.userName : "Unknown";
        }
    }
}
