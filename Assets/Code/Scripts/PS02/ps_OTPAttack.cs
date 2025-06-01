using UnityEngine;
using System.Collections;

[System.Serializable]
public class TimedCanvas
{
    public GameObject canvasObject;
    public float delay;
}

public class OTPAttack : MonoBehaviour
{
    public TimedCanvas[] timedCanvases;
    public Transform xrCamera;

    private bool hasStarted = false;

    void OnEnable()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            foreach (var canvas in timedCanvases)
            {
                StartCoroutine(ActivateCanvasAfterDelay(canvas.canvasObject, canvas.delay));
            }
        }
    }

    IEnumerator ActivateCanvasAfterDelay(GameObject canvas, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Check if any of the ending conditions are met before spawning the canvas, if so, cease spawning
        if (GameObject.Find("BadMath")?.activeSelf == true || GameObject.Find("GoodMath")?.activeSelf == true || GameObject.Find("MFA Breach Canvas")?.activeSelf == true || GameObject.Find("OTP Breach Canvas")?.activeSelf == true)
        {
            Debug.Log("An ending reached — not spawning " + canvas.name);
            yield break;
        }

        if (canvas != null && xrCamera != null)
        {
            Vector3 spawnPos = xrCamera.position + xrCamera.forward * 2f; // Spawn 2 meters in front of camera, at eye level
            spawnPos.y = xrCamera.position.y; // ensure eye-level placement

            Quaternion spawnRot = Quaternion.LookRotation(xrCamera.forward);

            canvas.transform.position = spawnPos;
            canvas.transform.rotation = spawnRot;

            canvas.SetActive(true);
            Debug.Log($"Spawned canvas: {canvas.name} in front of user at time {Time.time:F2}");
        }
        else
        {
            Debug.LogWarning("Canvas or XR Camera not assigned!");
        }
    }
}
