using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject infoPanel;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public Image statusIcon;

    void Start()
    {
        if (infoPanel != null)
            // infoPanel.SetActive(false); // Hide panel at start
            // Example: Show info panel at start with custom text and status
            ShowInfo("Welcome!", "This is the CyberSafeVR experience. Interact with objects to learn more.", "safe");
    }

    /// <summary>
    /// Show info panel with text and status color
    /// </summary>
    /// <param name="title">Object title</param>
    /// <param name="body">Object description</param>
    /// <param name="status">safe, risky, warning</param>
    public void ShowInfo(string title, string body, string status)
    {
        if (infoPanel == null || titleText == null || bodyText == null)
            return;

        infoPanel.SetActive(true);
        titleText.text = title;
        bodyText.text = body;

        if (statusIcon != null)
        {
            switch (status.ToLower())
            {
                case "safe":
                    statusIcon.color = Color.green; // #00FF00
                    break;
                case "risky":
                    statusIcon.color = Color.red;   // #FF0000
                    break;
                case "warning":
                    statusIcon.color = Color.yellow; // #FFFF00
                    break;
                default:
                    statusIcon.color = Color.white;
                    break;
            }
        }
    }

    public void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

}