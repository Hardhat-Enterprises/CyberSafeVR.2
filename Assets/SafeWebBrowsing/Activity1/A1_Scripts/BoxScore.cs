using UnityEngine;
using System.Collections.Generic;

public class BoxTriggerScorer : MonoBehaviour
{
    public string correctTag = "Safe";
    public ScoreManager scoreManager;

    // Tracks objects currently inside this box
    private HashSet<GameObject> trackedObjects = new HashSet<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        GameObject obj = other.gameObject;

        if (trackedObjects.Contains(obj)) return;

        if (obj.CompareTag(correctTag))
        {
            scoreManager.AddScore(1);
        }
        else
        {
            scoreManager.AddScore(-1);
        }

        trackedObjects.Add(obj);
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject obj = other.gameObject;

        if (!trackedObjects.Contains(obj)) return;

        if (obj.CompareTag(correctTag))
        {
            scoreManager.AddScore(-1);
        }
        else
        {
            scoreManager.AddScore(1);
        }

        trackedObjects.Remove(obj);
    }
}
