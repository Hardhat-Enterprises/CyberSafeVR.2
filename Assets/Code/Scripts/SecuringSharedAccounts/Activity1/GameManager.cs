using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SSA01
{
    public class GameManager : MonoBehaviour
    {
        public TeleportManager teleportManager; // Reference to the TeleportManager
        public Transform teleportDestination;
        public int endingType = 0;  // 0 = Good Ending, 1 = Bad Ending 1, 2 = Bad Ending 2, 3 = Bad Ending 3
        [Header("Ending Elements")]
        public GameObject[] ending0Elements; // Elements for Ending 0
        public GameObject[] ending1Elements; // Elements for Ending 1
        public GameObject[] ending2Elements; // Elements for Ending 2
        public GameObject[] ending3Elements; // Elements for Ending 3
        public GameObject[] badEndingElements; // Elements for the Bad Ending

        void Start()
        {
            if (teleportManager == null)
            {
                teleportManager = Object.FindFirstObjectByType<TeleportManager>();
            }
        }

        public void SetEnding(int ending)
        {
            endingType = ending;
        }

        public void FinishGame(){
            StartCoroutine(Finish());
        }

        public IEnumerator Finish(){
            yield return new WaitForSeconds(3f);
            teleportManager.Teleport(teleportDestination);
            switch (endingType)
            {
                case 0:
                    SetEndingElements(ending0Elements);
                    break;
                case 1:
                    SetEndingElements(ending1Elements);
                    SetEndingElements(badEndingElements);
                    break;
                case 2:
                    SetEndingElements(ending2Elements);
                    SetEndingElements(badEndingElements);
                    break;
                case 3:
                    SetEndingElements(ending3Elements);
                    SetEndingElements(badEndingElements);
                    break;
            }


        }

        void SetEndingElements(GameObject[] elementsToActivate)
        {
            // Activate the selected elements for the current ending
            foreach (GameObject element in elementsToActivate)
            {
                element.SetActive(true);
            }
        }
    }
}