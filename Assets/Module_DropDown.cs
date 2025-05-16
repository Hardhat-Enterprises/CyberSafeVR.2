using UnityEngine;
using System.Collections.Generic;

public class Module_DropDown : MonoBehaviour
{
    public List<GameObject> signs;          // Assign all sign GameObjects in order

    public void HandleInputData(int val)
    {
        if (val < 0 || val >= signs.Count)
        {
            Debug.LogWarning("Selected index is out of range.");
            return;
        }

        // Disable all signs
        foreach (GameObject sign in signs)
        {
            if (sign != null)
                sign.SetActive(false);
        }

        // Enable the selected sign
        GameObject selectedSign = signs[val];
        if (selectedSign != null)
        {
            selectedSign.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Selected sign is null.");
        }
    }
}
