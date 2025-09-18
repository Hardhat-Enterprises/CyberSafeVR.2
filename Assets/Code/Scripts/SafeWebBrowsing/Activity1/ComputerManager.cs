using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace SWB01{
    public class ComputerManager : MonoBehaviour
    {
        [Header("UI References")]
        public GroupManager groupManager;
        public TMP_Text descriptionText;
        public Button leftArrowButton;
        public Button rightArrowButton;

        [Header("Group Settings")]
        public string[] groupNames;
        public string[] groupDescriptions;
        public GameObject[] groupItems;

        private int currentGroupIndex = 0;

        private void Start()
        {
            // Set up button listeners
            leftArrowButton.onClick.AddListener(PreviousGroup);
            rightArrowButton.onClick.AddListener(NextGroup);
            
            // Initialize the first group
            UpdateGroupDisplay();
        }

        public void NextGroup()
        {
            currentGroupIndex = (currentGroupIndex + 1) % groupNames.Length;
            UpdateGroupDisplay();
        }

        public void PreviousGroup()
        {
            currentGroupIndex--;
            if (currentGroupIndex < 0) currentGroupIndex = groupNames.Length - 1;
            UpdateGroupDisplay();
        }

        private void UpdateGroupDisplay()
        {
            // Update group buttons
            if (groupManager != null)
            {
                groupManager.SetActiveGroup(groupNames[currentGroupIndex]);
            }

            // Update description text
            if (descriptionText != null && currentGroupIndex < groupDescriptions.Length)
            {
                descriptionText.text = groupDescriptions[currentGroupIndex];
            }

            // Update items visibility
            for (int i = 0; i < groupItems.Length; i++)
            {
                if (groupItems[i] != null)
                {
                    groupItems[i].SetActive(i == currentGroupIndex);
                }
            }
        }

        // Get current group info (optional)
        public string GetCurrentGroupName()
        {
            return groupNames[currentGroupIndex];
        }

        public string GetCurrentGroupDescription()
        {
            return groupDescriptions[currentGroupIndex];
        }
    }
}
