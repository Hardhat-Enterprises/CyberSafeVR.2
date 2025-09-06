using UnityEngine;

namespace SWB01{
    public class HighlightActivator : MonoBehaviour
    {
        public GameManager gameManager;
        private GameObject sibling; 
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (gameManager != null)
            {
                gameManager.onBadEnding.AddListener(CheckHighlight);
            }

            Transform parent = transform.parent;
            Transform siblingTransform = parent.Find("Highlight");

            if (siblingTransform != null){
                sibling = siblingTransform.gameObject;
            }
        }

        void OnDestroy(){
            if (gameManager != null)
            {
                gameManager.onBadEnding.RemoveListener(CheckHighlight);
            }
        }

        // Update is called once per frame
        void CheckHighlight()
        {
            bool safe = GetComponent<SocketLabelHandler>().isSafe;
            
            if (!safe && sibling != null)
            {
                sibling.SetActive(true);
            } else
            {
                Debug.Log("Highlight not found for " + gameObject.name 
                    + " (parent: " + (transform.parent != null ? transform.parent.name : "null") + ")");
            }
        }
    }

}
