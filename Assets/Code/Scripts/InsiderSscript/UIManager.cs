using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Info Panel")]
    [SerializeField] GameObject infoPanel;
    [SerializeField] Text titleText;
    [SerializeField] Text bodyText;
    [SerializeField] Image statusDot; // optional: green/ red

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ShowInfo(string title, string message, bool isSuspicious)
    {
        if (!infoPanel) return;
        infoPanel.SetActive(true);
        if (titleText) titleText.text = title;
        if (bodyText) bodyText.text = message;
        if (statusDot) statusDot.color = isSuspicious ? Color.red : Color.green;
    }

    public void HideInfo() { if (infoPanel) infoPanel.SetActive(false); }
}
1