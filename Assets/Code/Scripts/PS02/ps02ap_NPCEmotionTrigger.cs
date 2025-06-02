using UnityEngine;

public class NPCEmotionTrigger : MonoBehaviour
{
    public Animator npcAnimator;

    public void PlayAngryPoint()
    {
        npcAnimator.SetTrigger("AngryPoint");
    }

    public void PlayYelling()
    {
        npcAnimator.SetTrigger("Yelling");
    }

    public void PlayDefeated()
    {
        npcAnimator.SetTrigger("Defeated");
    }
}
