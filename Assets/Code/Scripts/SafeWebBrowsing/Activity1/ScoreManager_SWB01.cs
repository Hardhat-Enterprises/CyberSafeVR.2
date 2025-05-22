using UnityEngine;

public class ScoreManager_SWB01 : MonoBehaviour
{
    public int score = 0;
    public NarrationManager_SWB01 narrationManager;
    public string message = "Congratulations! You chose which site was safe successfully! Your Score is: ";
    public AudioClip messageAudio;
    public void AddScore(int value)
    {
        score += value;
        Debug.Log("Score: " + score);
    }

    public void AnnounceScore(){
        int final_score = score * 100 / 15;
        narrationManager.AddMessage(message + final_score + "/100");
        narrationManager.AddAudioClip(messageAudio);
    }
}
