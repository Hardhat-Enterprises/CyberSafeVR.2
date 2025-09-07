using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace SWB01
{
    public class AnswerHandler : MonoBehaviour
    {
        // Booleans for each socket
        public bool URL;
        public bool Encrypt;
        public bool Cert;
        public bool Homepage;
        public bool Login;
        public bool Ad;


        // Total count of "true" answers
        public int totalCount;
        public UnityEvent askEndGame;

        // Function to set a specific answer
        public void SetAnswer(string answerName, bool value)
        {
            switch (answerName)
            {
                case "URL":
                    URL = value;
                    break;
                case "Encrypt":
                    Encrypt = value;
                    break;
                case "Cert":
                    Cert = value;
                    break;
                case "Homepage":
                    Homepage = value;
                    break;
                case "Login":
                    Login = value;
                    break;
                case "Ad":
                    Ad = value;
                    break;
                default:
                    Debug.LogWarning("AnswerHandler: Unknown answer name: " + answerName);
                    break;
            }
        }

        public void CheckAnswers(){
            bool end = (URL && Encrypt && Cert && Homepage && Login && Ad);
            GameManager gameManager = GetComponent<GameManager>();
            Debug.Log("ending");
            gameManager.SetEnding(end);
            gameManager.FinishGame();
        }

        // Function to increment total count
        public void IncrementTotal(int amount = 1)
        {
            totalCount += amount;
            if(totalCount == 6){
                askEndGame?.Invoke();
            }
        }

        // Function to decrement total count
        public void DecrementTotal(int amount = 1)
        {
            totalCount -= amount;
            if (totalCount < 0)
                totalCount = 0; // Prevent negative count
        }

        // Optional: function to reset all answers
        public void ResetAnswers()
        {
            URL = Encrypt = Cert = Homepage = Login = Ad = false;
            totalCount = 0;
        }

    }
}
