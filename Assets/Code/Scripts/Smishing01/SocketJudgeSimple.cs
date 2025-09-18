using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public enum ExpectedType { Phishing, Safe }

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor))]
public class SocketJudgeSimple : MonoBehaviour
{
    [Header("What this socket expects")]
    public ExpectedType expectedType;

    [Header("Feedback UI")]
    public TMP_Text feedbackText;           // drag a TMP label here (shared or per-zone)
    public string correctMsg = "Correct!";
    public string wrongMsg = "Wrong side!";
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public float showSeconds = 1.2f;

    UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket;
    Coroutine hideCo;

    void Awake()
    {
        socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        socket.selectEntered.AddListener(OnCardDropped);
    }

    void OnCardDropped(SelectEnterEventArgs args)
    {
        var tag = args.interactableObject.transform.GetComponent<EmailCardTag>();
        if (!tag) return;

        bool correct =
            (tag.isPhishing && expectedType == ExpectedType.Phishing) ||
            (!tag.isPhishing && expectedType == ExpectedType.Safe);

        ShowFeedback(correct);
    }

    void ShowFeedback(bool correct)
    {
        if (!feedbackText) return;

        feedbackText.text = correct ? correctMsg : wrongMsg;
        feedbackText.color = correct ? correctColor : wrongColor;
        feedbackText.gameObject.SetActive(true);

        if (hideCo != null) StopCoroutine(hideCo);
        hideCo = StartCoroutine(HideAfterDelay());
    }

    System.Collections.IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showSeconds);
        feedbackText.gameObject.SetActive(false);
    }
}
