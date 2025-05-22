using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class LoadSceneButton_SSA01 : MonoBehaviour
{

    // Method to call when the button is clicked
    public void LoadScene(string sceneName)
    {
        // Load the specified scene by its name
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Debug.Log("Quitting Game...");

        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
