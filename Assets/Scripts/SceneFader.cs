using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneFader : MonoBehaviour
{
    [Header("Fader Settings")]
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("Loading Text")]
    public TextMeshProUGUI loadingText;

    private void Start()
    {
        if (loadingText != null)
            loadingText.gameObject.SetActive(false);

        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        fadeImage.color = new Color(0, 0, 0, 1);
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        float timer = 0f;
        fadeImage.color = new Color(0, 0, 0, 0);
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = timer / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);

        if (loadingText != null)
            loadingText.gameObject.SetActive(true);

        SceneManager.LoadScene(sceneName);
    }
}