using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    [Header("Fader Settings")]
    public Image fadeImage;          // The black overlay image
    public float fadeDuration = 1f;  // How long the fade takes

    private void Start()
    {
        // Fade in when the scene loads
        StartCoroutine(FadeIn());
    }

    // Call this from your button — pass in the scene name as a string
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        fadeImage.color = new Color(0, 0, 0, 1); // Start fully black

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0); // Fully transparent
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float timer = 0f;
        fadeImage.color = new Color(0, 0, 0, 0); // Start transparent

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1); // Fully black
        SceneManager.LoadScene(sceneName);        // Load the next scene
    }
}