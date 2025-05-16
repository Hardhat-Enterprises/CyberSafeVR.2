using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Activity_DropDown : MonoBehaviour
{
    public List<string> scene_names;
    private TMP_Dropdown dropdown;

    void Start()
    {
        dropdown = GetComponentInParent<TMP_Dropdown>();

        if (dropdown != null && dropdown.options.Count > 0)
        {
            HandleInputData(dropdown.value);
        }
        else
        {
            Debug.LogWarning("TMP_Dropdown is missing or has no options.");
        }
    }

    public void HandleInputData(int val)
    {
        if (val < 0 || val >= scene_names.Count)
        {
            Debug.LogWarning("Selected index is out of range.");
            return;
        }

        string selectedScene = scene_names[val];
        Debug.Log("Selected scene: " + selectedScene);

        if (SceneSelectionManager.Instance != null)
        {
            SceneSelectionManager.Instance.SetSelectedScene(selectedScene);
        }
        else
        {
            Debug.LogError("SceneSelectionManager instance is null!");
        }
    }
}
