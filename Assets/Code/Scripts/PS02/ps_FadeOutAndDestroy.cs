using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeOutAndDestroy : MonoBehaviour
{
    public float fadeDuration = 1.5f;
    public AudioClip disappearSound;
    public float shrinkScale = 0.3f;

    private CanvasGroup canvasGroup;
    private AudioSource audioSource;
    private Vector3 originalScale;

    void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();

        if (canvasGroup == null)
            Debug.LogWarning("?? No CanvasGroup found for fadeout!");

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>(); // fallback if not set

        originalScale = transform.localScale;
    }

    public void StartFadeOut()
    {
        if (canvasGroup != null)
            StartCoroutine(FadeOutRoutine());
        else
            Destroy(gameObject); // fallback
    }

    IEnumerator FadeOutRoutine()
    {
        if (disappearSound != null)
            audioSource.PlayOneShot(disappearSound);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Fade canvas alpha
            canvasGroup.alpha = 1 - t;

            // Shrink scale
            transform.localScale = Vector3.Lerp(originalScale, originalScale * shrinkScale, t);

            yield return null;
        }

        canvasGroup.alpha = 0;
        gameObject.SetActive(false); // or Destroy(gameObject) sometimes does not work?? better to SetActive (false);
    }
}
