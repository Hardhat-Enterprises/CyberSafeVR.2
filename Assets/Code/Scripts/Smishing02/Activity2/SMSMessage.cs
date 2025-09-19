using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace SMS02
{
    public class SMSMessage : MonoBehaviour
    {
        public TextMeshPro messageText;
        public XRBaseInteractable safeButton;
        public XRBaseInteractable smishButton;
        public bool isSmishing;

        void OnEnable()
        {
            // Updated event hookup using SelectEnterEventArgs
            safeButton.selectEntered.AddListener(OnSafeSelected);
            smishButton.selectEntered.AddListener(OnSmishSelected);
        }

        private void OnSafeSelected(SelectEnterEventArgs args)
        {
            CheckAnswer(false); // Player thinks it's safe
        }

        private void OnSmishSelected(SelectEnterEventArgs args)
        {
            CheckAnswer(true); // Player thinks it's smishing
        }

        void CheckAnswer(bool userChoseSmish)
        {
            bool correct = userChoseSmish == isSmishing;
            ScoreManager.Instance.AddScore(correct);

            if (correct)
                Debug.Log("Correct!");
            else
                Debug.Log("Wrong! Breach simulated!");

            Destroy(gameObject, 0.2f);
        }
    }
}
