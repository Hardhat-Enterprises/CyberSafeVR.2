using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    private InputDevice leftHandDevice;
    private InputDevice rightHandDevice;

    void Start()
    {
        // Initialize devices at start
        InitializeDevices();
    }

    void OnEnable()
    {
        InputDevices.deviceConnected += OnDeviceConnected;
    }

    void OnDisable()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
    }

    void Update()
    {
        if (IsPauseButtonPressed())
        {
            TogglePauseMenu();
        }
    }

    private void InitializeDevices()
    {
        leftHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        rightHandDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
    }

    private void OnDeviceConnected(InputDevice device)
    {
        // Re-initialize if a new device is connected
        InitializeDevices();
    }

    private bool IsPauseButtonPressed()
    {
        // Editor simulation
        if (Input.GetKeyDown(KeyCode.M))
            return true;

        bool leftPressed = leftHandDevice.isValid &&
                           leftHandDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool lp) && lp;

        bool rightPressed = rightHandDevice.isValid &&
                            rightHandDevice.TryGetFeatureValue(CommonUsages.menuButton, out bool rp) && rp;

        return leftPressed || rightPressed;
    }

    public void TogglePauseMenu()
    {
        if (isPaused)
            ResumeGame();
        else
            ShowPauseMenu();
    }

    public void ShowPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
