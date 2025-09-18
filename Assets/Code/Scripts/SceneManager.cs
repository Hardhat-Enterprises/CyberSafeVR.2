using UnityEngine;

public class SceneSelectionManager : MonoBehaviour
{
    public static SceneSelectionManager Instance { get; private set; }

    public string selectedScene;
    private SceneLoader sceneLoader;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        sceneLoader = GetComponent<SceneLoader>();

        if (sceneLoader == null)
        {
            Debug.LogError("SceneLoader component not found on the same GameObject.");
        }
    }

    public void SetSelectedScene(string sceneName)
    {
        selectedScene = sceneName;
    }

    public void StartSelectedScene()
    {
        if (string.IsNullOrEmpty(selectedScene))
        {
            selectedScene="SafeWebBrowsing01";
        }

        if (sceneLoader != null)
        {
            try
            {
                sceneLoader.LoadScene(selectedScene);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load scene: " + selectedScene + "\n" + e.Message);
            }
        }
    }
}
