using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pausePanel;

    [Header("Input")]
    public InputActionReference pauseButton;

    private bool isPaused = false;

    private void OnEnable()
    {
        pauseButton.action.Enable();
        pauseButton.action.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        pauseButton.action.performed -= OnPausePressed;
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    // TEMP: keyboard test
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResumeGame();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Position panel in front of camera
            Camera cam = Camera.main;
            pausePanel.transform.position = cam.transform.position + cam.transform.forward * 2f;
            pausePanel.transform.rotation = Quaternion.LookRotation(cam.transform.forward);
        }

        pausePanel.SetActive(isPaused);
    }

    public void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
    }

    public void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}