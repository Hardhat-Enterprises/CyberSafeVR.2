using UnityEngine;

public class OpenApp : MonoBehaviour
{
    [Header("Panels")]
    public GameObject desktopWallpaper;  // your root for the desktop UI
    public GameObject cloudApp;          // your root for the cloud app UI

    void Start()
    {
        // Ensure initial state: desktop on, cloud off
        if (desktopWallpaper != null) desktopWallpaper.SetActive(true);
        if (cloudApp         != null) cloudApp.SetActive(false);
    }

    // Hook this up to your CloudButton.OnClick
    public void ShowCloudApp()
    {
        if (desktopWallpaper != null) desktopWallpaper.SetActive(false);
        if (cloudApp         != null) cloudApp.SetActive(true);
    }

    // (Optional) if you have a Back button on CloudApp:
    /*public void ShowDesktop()
    {
        if (cloudApp         != null) cloudApp.SetActive(false);
        if (desktopWallpaper != null) desktopWallpaper.SetActive(true);
    }
    */
}
