using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace CATM_WC{
    public class GameManager : MonoBehaviour
    {
        [Header("Timeline State")]
        [Tooltip("0 = Past, 1 = Future")]
        public int timelineState = 0;
        public int saved = 0;

        private int cards = 0;

        [Header("NPC Controller")]
        public CW_NPCController npcController;

        [Header("Fix Events (Assign in Inspector)")]
        public UnityEvent onDone;
        public UnityEvent onBackToFuture;
        public UnityEvent onBackToPast;
        public UnityEvent onCardsFixed;

        [Header("Disaster Signs")]
        public List<GameObject> disasterSigns;  // List to hold disaster sign objects

        // Public methods you can call from anywhere (buttons, colliders, triggers)
        public void FixUpdate()
        {
            saved++;
            TestConds();

        }

        public void FixPhishing()
        {
            saved++;
            TestConds();
        }

        public void FixBackup()
        {
            saved++;
            TestConds();
        }

        public void Future()
        {
            onBackToFuture?.Invoke();
        }

        public void Past()
        {
            DeactivateDisasterSigns();
            onBackToPast?.Invoke();
        }

        public void TestConds(){
            if(saved == 3){
                onDone?.Invoke();
            }
        }

        public void CheckCardForDoor(){
            cards++;
            if(cards == 3){
                onCardsFixed?.Invoke();
            }
        }

        // Optional utility if needed
        public void SetTimelineState(int state)
        {
            timelineState = Mathf.Clamp(state, 0, 1);
        }

        // Methods to activate and deactivate disaster signs
        public void ActivateDisasterSigns()
        {
            foreach (var sign in disasterSigns)
            {
                if (sign != null)
                    sign.SetActive(true);  // Activate each sign in the list
            }
        }

        public void DeactivateDisasterSigns()
        {
            foreach (var sign in disasterSigns)
            {
                if (sign != null)
                    sign.SetActive(false);  // Deactivate each sign in the list
            }
        }
    }
}