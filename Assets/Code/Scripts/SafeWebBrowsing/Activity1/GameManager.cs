using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


namespace SWB01
{
    public class GameManager: MonoBehaviour
    {
        public int endingType = 0; // 0 = Good, 1 = Bad

        [System.Serializable]
        public class EndingEventGroup
        {
            public List<UnityEvent> events = new List<UnityEvent>();
        }

        [Header("Ending Event Groups")]
        public EndingEventGroup goodEndingEvents;
        public EndingEventGroup badEndingEvents;
        public EndingEventGroup endingEvents;

        public void SetEnding(int ending)
        {
            endingType = ending;
        }

        public void FinishGame()
        {
            
            switch (endingType)
            {
                case 0:
                    InvokeEventGroup(goodEndingEvents);
                    break;
                case 1:
                    InvokeEventGroup(badEndingEvents);
                    break;
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