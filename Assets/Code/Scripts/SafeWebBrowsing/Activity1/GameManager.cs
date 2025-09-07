using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


namespace SWB01
{
    public class GameManager: MonoBehaviour
    {
        public bool endingType = false; // 0 = Good, 1 = Bad

        [System.Serializable]
        public class EndingEventGroup
        {
            public List<UnityEvent> events = new List<UnityEvent>();
        }

        public UnityEvent onBadEnding;
        
        [Header("Ending Event Groups")]
        public EndingEventGroup goodEndingEvents;
        public EndingEventGroup badEndingEvents;
        public EndingEventGroup endingEvents;

        public void SetEnding(bool ending)
        {
            endingType = ending;
        }

        public void FinishGame()
        {
            if(endingType){
                InvokeEventGroup(goodEndingEvents);
            }else{
                InvokeEventGroup(badEndingEvents);
                onBadEnding?.Invoke();
            }
            
            InvokeEventGroup(endingEvents);
        }

        void InvokeEventGroup(EndingEventGroup group)
        {
            foreach (var unityEvent in group.events)
            {
                unityEvent.Invoke();
            }
        }
    }
}