using UnityEngine;
using System.Collections.Generic;
using System;

public class AccessListController_SSA1 : MonoBehaviour
{
    public GameManager_SSA1 GameManager;
    public FloatingNarrationManager_SSA01 Narration;
    public List<string> activeUsers = new List<string>();
    private List<string> validNames = new List<string> { "John Doe", "Alex Brown", "Bob Cross" };

    public GameObject spotlight;  // Drag your spotlight GameObject here
    public event Action OnUserListChanged;

    public void AddUser(string name)
    {
        if (!activeUsers.Contains(name))
        {
            activeUsers.Add(name);
            OnUserListChanged?.Invoke();
        }
    }

    public void RemoveUser(string name)
    {
        if (activeUsers.Contains(name))
        {
            activeUsers.Remove(name);
            OnUserListChanged?.Invoke();
        }
    }

    public List<string> GetUsers()
    {
        return new List<string>(activeUsers);
    }

    // Function to check if the user names are valid
    public bool IsValidUser(string name)
    {
        return validNames.Contains(name);
    }

    // Function to check the entire list of users
    public void ValidateUserList()
    {
        int count=0;
        foreach (var user in activeUsers)
        {
            if (!IsValidUser(user))
            {
                Debug.LogWarning($"Invalid user: {user}");
                GameOver(1);
                return;
            }
            count++;
        }
        if(count < 3){
            GameOver(2);
        } else {
            ShowNextMessages();
        }
    }

    private void ShowNextMessages(){
        Narration.AddMessage("Now that we have managed the access list, we need to make sure the account is secure by changing the password.");
        Narration.AddMessage("Approach the screen to exit the Password Manager window and use the browser to log in to your shared account.");
        Narration.NextMessage();
    }
    // Activates the spotlight for invalid users
    private void ActivateSpotlight()
    {
        if (spotlight != null)
        {
            spotlight.SetActive(true);  // Activate spotlight
        }
    }

    private void GameOver(int num)
    {
        ActivateSpotlight();
        GameManager.SetEnding(num);
        GameManager.FinishGame();
    }

}
