using UnityEngine;

namespace CATM_WC{
    public class NarrationTrigger : MonoBehaviour
    {
        public GameObject narrationTrigger;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && narrationTrigger != null)
            {
                narrationTrigger.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && narrationTrigger != null)
            {
                narrationTrigger.SetActive(false);
            }
        }
    }
}