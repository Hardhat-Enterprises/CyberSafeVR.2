using UnityEngine;

namespace IT01
{
    public class DoorTriggerController : MonoBehaviour
    {
        [Header("Door Animator")]
        [SerializeField] private Animator doorAnimator;

        [Header("Settings")]
        [SerializeField] private bool openOnTrigger = true;
        [SerializeField] private bool closeOnExit = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && openOnTrigger)
            {
                doorAnimator.Play("DoorOpen", 0, 0f);
                Debug.Log("Door opening...");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && closeOnExit)
            {
                doorAnimator.Play("DoorClose", 0, 0f);
                Debug.Log("Door closing...");
            }
        }
    }
}
