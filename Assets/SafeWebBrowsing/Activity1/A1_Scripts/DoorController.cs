using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;
    public Vector3 rotationAxis = Vector3.up;
    public float animationDuration = 1.0f;
    public float scaleFactor = 0.8f;

    [Header("Audio Clips")]
    public AudioClip openClip;
    public AudioClip closeClip;
    public AudioClip lockedClip;

    private bool isOpen = false;
    private bool isAnimating = false;
    private bool isLocked = false;

    private Coroutine doorRoutine;
    private Quaternion initialRotation;
    private Vector3 initialScale;
    private AudioSource audioSource;

    private void Awake()
    {
        initialRotation = transform.localRotation;
        initialScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
    }

    public void ToggleDoor()
    {
        if (isAnimating)
            return;

        if (isLocked)
        {
            PlaySound(lockedClip);
            return;
        }

        isOpen = !isOpen;

        if (doorRoutine != null)
            StopCoroutine(doorRoutine);

        if (isOpen)
        {
            PlaySound(openClip); // Play open sound immediately
        }

        doorRoutine = StartCoroutine(AnimateDoor(isOpen));
    }

    private IEnumerator AnimateDoor(bool open)
    {
        isAnimating = true;

        Quaternion startRotation = transform.localRotation;
        Quaternion endRotation = open
            ? initialRotation * Quaternion.AngleAxis(openAngle, rotationAxis)
            : initialRotation;

        Vector3 startScale = transform.localScale;
        Vector3 endScale = open ? GetScaledSizeForOpening() : initialScale;

        float time = 0;
        while (time < animationDuration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / animationDuration;

            transform.localRotation = Quaternion.Slerp(startRotation, endRotation, normalizedTime);
            transform.localScale = Vector3.Lerp(startScale, endScale, normalizedTime);

            yield return null;
        }

        transform.localRotation = endRotation;
        transform.localScale = endScale;

        if (!open)
        {
            PlaySound(closeClip); // Play close sound at the end
        }

        isAnimating = false;
    }

    private Vector3 GetScaledSizeForOpening()
    {
        Vector3 scale = initialScale;
        scale.x = initialScale.x * scaleFactor;
        return scale;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void LockDoor(bool locked)
    {
        isLocked = locked;
    }
}
