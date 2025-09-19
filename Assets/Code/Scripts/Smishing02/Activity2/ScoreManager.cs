using UnityEngine;

namespace SMS02
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance;
        public int correctCount = 0;
        public int incorrectCount = 0;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void AddScore(bool correct)
        {
            if (correct) correctCount++;
            else incorrectCount++;
        }
    }
}
