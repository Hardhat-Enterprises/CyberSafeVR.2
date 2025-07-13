using UnityEngine;
using UnityEngine.SceneManagement;

namespace PS02
{
    public class RestartScene : MonoBehaviour
    {
        public void RestartCurrentScene()
        {
            // Reload the active scene
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}