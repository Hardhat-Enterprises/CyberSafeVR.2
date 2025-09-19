using UnityEngine;
using TMPro;

namespace InsiderThreat01 {
    public class DropdownValueRouter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Dropdown dropdown;     // assign your TMP_Dropdown
        [SerializeField] private GameObject backOfAutofills;
        [SerializeField] private GameObject akashPanel;
        [SerializeField] private GameObject andrePanel;

        // If you ever reorder options, update these to match the indices in your TMP_Dropdown
        [Header("Option Indices in TMP_Dropdown")]
        [SerializeField] private int idxAutofill = 0;
        [SerializeField] private int idxAkash = 1;
        [SerializeField] private int idxAndre = 2;

        void Awake()
        {
            if (dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
        }

        void Start()
        {
            // Ensure a clean starting state and react to current value
            ApplyIndex(dropdown != null ? dropdown.value : idxAutofill);
        }

        // Hook this in the Inspector: TMP_Dropdown -> On Value Changed (Int32) -> DropdownValueRouter.OnValueChanged
        public void OnValueChanged(int index)
        {
            ApplyIndex(index);
        }

        private void ApplyIndex(int index)
        {
            // default: everything off
            if (backOfAutofills) backOfAutofills.SetActive(false);
            if (akashPanel) akashPanel.SetActive(false);
            if (andrePanel) andrePanel.SetActive(false);

            // turn on based on selection
            if (index == idxAkash)
            {
                if (backOfAutofills) backOfAutofills.SetActive(true);
                if (akashPanel) akashPanel.SetActive(true);
            }
            else if (index == idxAndre)
            {
                if (backOfAutofills) backOfAutofills.SetActive(true);
                if (andrePanel) andrePanel.SetActive(true);
            }
            // if Autofill (idxAutofill) -> keep all off
        }
    }
}

