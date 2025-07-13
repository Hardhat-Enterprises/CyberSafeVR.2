using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace SSA01
{
    public class AccessListDisplay : MonoBehaviour
    {
        public AccessListController accessListController; // drag your controller here
        public Transform contentContainer; // drag your UI Content object here
        public GameObject nameTextPrefab; // drag a prefab with a Text component here

        private GameObject janeSmithRow; // Persistent row for Jane Smith
        private GameObject addUserRow; // Persistent "+ Add User" row

        void Start()
        {
            accessListController.OnUserListChanged += RefreshDisplay;
            CreateJaneSmithRow(); // Only once
            CreateAddUserRow();   // Only once
            RefreshDisplay();
        }

        void OnDestroy()
        {
            // Always unsubscribe to avoid memory leaks or null references
            if (accessListController != null)
                accessListController.OnUserListChanged -= RefreshDisplay;
        }

        void CreateJaneSmithRow()
        {
            if (janeSmithRow == null)
            {
                janeSmithRow = Instantiate(nameTextPrefab, contentContainer);
                TMP_Text name = janeSmithRow.transform.Find("NameText").GetComponent<TMP_Text>();
                TMP_Text role = janeSmithRow.transform.Find("RoleText").GetComponent<TMP_Text>();
                TMP_Text remove = janeSmithRow.transform.Find("RemoveText").GetComponent<TMP_Text>();

                name.text = "Jane Smith";
                role.text = "Admin";
                remove.text = "Leave";
            }
        }

        void CreateAddUserRow()
        {
            if (addUserRow == null)
            {
                addUserRow = Instantiate(nameTextPrefab, contentContainer);
                TMP_Text name = addUserRow.transform.Find("NameText").GetComponent<TMP_Text>();
                TMP_Text role = addUserRow.transform.Find("RoleText").GetComponent<TMP_Text>();
                TMP_Text remove = addUserRow.transform.Find("RemoveText").GetComponent<TMP_Text>();

                name.text = "";
                role.text = "";
                remove.text = "+ Add User";
            }
        }

        public void RefreshDisplay()
        {
            // First, make sure the persistent rows are not destroyed
            CreateJaneSmithRow();
            CreateAddUserRow();

            // Clear other display (but keep the persistent rows)
            foreach (Transform child in contentContainer)
            {
                if (child != janeSmithRow.transform && child != addUserRow.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            // Get current list of users and display them
            List<string> users = accessListController.GetUsers();
            foreach (var name in users)
            {
                Debug.Log("Adding name: " + name);  // Debugging line

                // Instantiate the row prefab
                GameObject row = Instantiate(nameTextPrefab, contentContainer);

                // Get the NameText child object
                TMP_Text nameText = row.transform.Find("NameText").GetComponent<TMP_Text>();

                // Set the name text
                nameText.text = name;
            }

            // Always move Jane Smith's row to the top
            janeSmithRow.transform.SetAsFirstSibling();
            
            // Move "+ Add User" row to the bottom
            addUserRow.transform.SetAsLastSibling();
        }
    }
}