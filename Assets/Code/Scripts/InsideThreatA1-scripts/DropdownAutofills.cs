using UnityEngine;
using TMPro;

namespace InsiderThreat01
{
    public class DropdownAutofills : MonoBehaviour
    {
        [Header("UI")]
        public TMP_Dropdown dropdown;
        public GameObject backOfAutofills;

        [Header("User Panels")]
        public GameObject akashPanel;
        public GameObject andrePanel;

        [Header("Option Texts (must match dropdown option text)")]
        public string optionAutofill = "Autofill";
        public string optionAkash = "Akash";
        public string optionAndre = "Andre";

        void Awake()
        {
            if (dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
        }

        void Start()
        {
            SetAllOff();
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
            OnDropdownChanged(dropdown.value);  // run once at start
        }

        void OnDropdownChanged(int index)
        {
            string choice = dropdown.options[index].text;
            SetAllOff();

            if (choice == optionAkash)
            {
                backOfAutofills.SetActive(true);
                akashPanel.SetActive(true);
            }
            else if (choice == optionAndre)
            {
                backOfAutofills.SetActive(true);
                andrePanel.SetActive(true);
            }
            // if Autofill → keep everything off
        }

        void SetAllOff()
        {
            if (backOfAutofills) backOfAutofills.SetActive(false);
            if (akashPanel) akashPanel.SetActive(false);
            if (andrePanel) andrePanel.SetActive(false);
        }
    }
}


