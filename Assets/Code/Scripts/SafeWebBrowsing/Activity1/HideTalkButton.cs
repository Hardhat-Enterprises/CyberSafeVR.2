using UnityEngine;

namespace SWB01{
    public class HideTalkButton : MonoBehaviour
    {
        public GameObject objectToHide;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            objectToHide.SetActive(false);
        }

    }

}
