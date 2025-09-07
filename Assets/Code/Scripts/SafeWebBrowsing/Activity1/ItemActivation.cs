using UnityEngine;
using UnityEngine.Events;

namespace SWB01
{
    public class ItemActivationEvent : MonoBehaviour
    {
        [Header("Event to trigger when item is activated")]
        public UnityEvent onActivated;

        void OnEnable()
        {
            onActivated?.Invoke();
        }
    }
}
