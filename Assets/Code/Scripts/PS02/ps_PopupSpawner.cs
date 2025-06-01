using UnityEngine;
using System.Collections.Generic;

public class PopupSpawner : MonoBehaviour
{
    public GameObject popupPrefab;
    public Transform xrRig;

    [Header("Timing")]
    public float initialInterval = 30f; // Pop up start after 30 seconds
    public float accelerationRate = 0.7f; // How much to reduce the interval each time
    public float minInterval = 1f; // Minimum interval between popups
    public int maxPopups = 45; // Maximum number of popups allowed

    private float nextSpawnTime;
    private float currentInterval;
    private bool stopSpawning = false;
    private int popupCount = 0;


    private List<GameObject> activePopups = new List<GameObject>();

    void Start()
    {
        currentInterval = initialInterval;
        nextSpawnTime = Time.time + currentInterval;
    }

    void Update()
    {
        if (stopSpawning || GameObject.Find("OTP Breach Canvas")?.activeSelf == true) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnPopup();
            currentInterval = Mathf.Max(minInterval, currentInterval * accelerationRate);
            nextSpawnTime = Time.time + currentInterval;
        }
    }

    public void StopSpawning()
    {
        Debug.Log("STOPPING SPAWN — flag flipped and invoke canceled");
        stopSpawning = true;

        // Find all active popups
        GameObject[] allPopups = GameObject.FindGameObjectsWithTag("Popup"); // Tag them "Popup"

        foreach (GameObject popup in allPopups)
        {
            FadeOutAndDestroy fader = popup.GetComponent<FadeOutAndDestroy>();
            if (fader != null)
            {
                fader.StartFadeOut(); // This coroutine will handle fading + destroying
            }
            else
            {
                // If no fader, just destroy normally
                Destroy(popup);
            }
        }
    }


    void SpawnPopup()
    {
        if (stopSpawning)
        {
            Debug.LogWarning("WARNING: SpawnPopup() was called even though spawning was stopped!");
            return;
        }

        if (popupCount >= maxPopups)
        {
            Debug.Log("Popup limit reached. Stopping spawn.");
            stopSpawning = true;
            return;
        }

        Debug.Log("Spawning popup");

        // Position in front of the XR rig
        Vector3 spawnPos = xrRig.position + xrRig.forward * 2f;
        spawnPos += new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.3f, 0.5f),
            0f
        );

        Quaternion spawnRot = Quaternion.LookRotation(xrRig.forward);
        GameObject popup = Instantiate(popupPrefab, spawnPos, spawnRot);

        // Track the spawned popup
        activePopups.Add(popup);

        popupCount++;

        // Hook up root reference
        var controller = popup.GetComponentInChildren<PopupTextController>();
        if (controller != null)
        {
            controller.popupRoot = popup;
        }
    }
}
