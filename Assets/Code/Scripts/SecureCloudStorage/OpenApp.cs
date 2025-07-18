using UnityEngine;

namespace SCS_DS
{
    public class OpenApp : MonoBehaviour
    {
        public GameObject desktopWallpaper;
        public GameObject cloudApp;

        void Start()
        {
            desktopWallpaper?.SetActive(true);
            cloudApp?.SetActive(false);
        }

        public void ShowCloudApp()
        {
            desktopWallpaper?.SetActive(false);
            cloudApp?.SetActive(true);
        }
    }
}
