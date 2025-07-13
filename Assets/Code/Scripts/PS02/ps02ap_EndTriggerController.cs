using UnityEngine;

public class EndTriggerController : MonoBehaviour
{
    public FrustrationManager frustrationManager;
    public SuspicionManager suspicionManager;

    public GameObject winEndCanvas;
    public GameObject loseEndCanvas;

    public Animator npcAnimator; // To control NPC animations

    private bool winTriggered = false;
    private bool loseTriggered = false;

    private void Update()
    {
        if (!winTriggered && frustrationManager.currentFrustration >= frustrationManager.maxFrustration)
        {
            TriggerGoodEnd();
        }

        if (!loseTriggered && suspicionManager.currentSuspicion >= suspicionManager.maxSuspicion)
        {
            TriggerBadEnd();
        }
    }

    void TriggerGoodEnd()
    {
        winTriggered = true;
        Debug.Log("User is frustrated! Win!");
        if (winEndCanvas != null) winEndCanvas.SetActive(true);

        if (npcAnimator != null)
        {
            npcAnimator.SetTrigger("Praying"); // Trigger NPC sad animation
        }

    }

    void TriggerBadEnd()
    {
        loseTriggered = true;
        Debug.Log("User gets suspicious! Lose!");
        if (winEndCanvas != null) loseEndCanvas.SetActive(true);

        if (npcAnimator != null)
        {
            npcAnimator.SetTrigger("JoyfulJump"); // Trigger NPC happy animation
        }
    }
}