using UnityEngine;
using UnityEngine.UI;

public class AccountOptionController : MonoBehaviour
{
    [Header("Buttons")]
    public Button AccountButton;     // CloudApp/Panel/Account (Button)
    public Button PublicButton;      // CloudApp/Panel/Account/OptionalPanel/PublicButton
    public Button PrivateButton;     // CloudApp/Panel/Account/OptionalPanel/PrivateButton

    [Header("Panels")]
    public GameObject OptionalPanel; // CloudApp/Panel/Account/OptionalPanel
    public GameObject FeedbackPanel; // CloudApp/Panel/Account/FeedbackPanel

    [Header("Feedback Texts")]
    public GameObject goodText;      // CloudApp/Panel/Account/FeedbackPanel/Good!
    public GameObject incorrectText; // CloudApp/Panel/Account/FeedbackPanel/Incorrect

    void Start()
    {
        // 1) hide sub‐panels at start
        OptionalPanel.SetActive(false);
        FeedbackPanel.SetActive(false);

        // 2) wire up your button clicks
        AccountButton.onClick   .AddListener(OnAccountClicked);
        PublicButton.onClick    .AddListener(OnPublicClicked);
        PrivateButton.onClick   .AddListener(OnPrivateClicked);
    }

    void OnAccountClicked()
    {
        // show the two choices
        FeedbackPanel.SetActive(false);
        OptionalPanel.SetActive(true);
    }

    void OnPublicClicked()
    {
        // hide optional, show incorrect
        OptionalPanel.SetActive(false);
        goodText.SetActive(false);
        incorrectText.SetActive(true);
        FeedbackPanel.SetActive(true);
    }

    void OnPrivateClicked()
    {
        // hide optional, show good
        OptionalPanel.SetActive(false);
        incorrectText.SetActive(false);
        goodText.SetActive(true);
        FeedbackPanel.SetActive(true);
    }
}
