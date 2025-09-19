using UnityEngine;
using UnityEngine.EventSystems;

namespace InsiderThreat01
{
    public class DesktopClicker : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        [SerializeField] private GameObject modalBlocker;
        [SerializeField] private GameObject passwordWindow;

        void Update()
        {
            // Don’t trigger clicks if pointer is over UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                if (!cam) cam = Camera.main;
                if (!cam) return;

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("Monitor") || hit.collider.CompareTag("StickyNote"))
                    {
                        OpenPasswords();
                    }
                }
            }
        }

        public void OpenPasswords()
        {
            if (modalBlocker) modalBlocker.SetActive(true);
            if (passwordWindow) passwordWindow.SetActive(true);
        }

        public void CloseAll()
        {
            if (modalBlocker) modalBlocker.SetActive(false);
            if (passwordWindow) passwordWindow.SetActive(false);
        }
    }
}


