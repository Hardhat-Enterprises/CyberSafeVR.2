using UnityEngine;
using TMPro;

namespace Menu
{
    public class DropdownAutoSelect : MonoBehaviour
    {
        private TMP_Dropdown dropdown;
        private Activity_DropDown activityDropDown;

        void Awake()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            activityDropDown = GetComponentInChildren<Activity_DropDown>();

            if (dropdown == null)
            {
                Debug.LogError("TMP_Dropdown component not found on this GameObject.");
            }

            if (activityDropDown == null)
            {
                Debug.LogError("Activity_DropDown script not found in children.");
            }
        }

        void OnEnable()
        {
            if (dropdown == null || activityDropDown == null)
                return;

            if (dropdown.options.Count > 1)
            {
                dropdown.value = 1;
                dropdown.RefreshShownValue();
                activityDropDown.HandleInputData(1);
            }
            else if (dropdown.options.Count > 0)
            {
                dropdown.value = 0;
                dropdown.RefreshShownValue();
                activityDropDown.HandleInputData(0);
            }
            else
            {
                Debug.LogWarning("TMP_Dropdown has no options.");
            }
        }
    }
}