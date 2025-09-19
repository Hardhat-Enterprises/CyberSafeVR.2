using UnityEngine;

namespace InsiderThreat01 {
    public class FeedbackHandler : MonoBehaviour
    {
        [Header("Next Quiz Panel (leave empty if last)")]
        public GameObject nextQuiz;

        public void GoNext()
        {
            gameObject.SetActive(false);

            if (nextQuiz != null)
            {
                nextQuiz.SetActive(true);
            }
            else
            {
                Debug.Log("Quiz sequence finished!");
            }
        }
    }
}

