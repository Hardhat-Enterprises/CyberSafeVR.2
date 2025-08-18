using UnityEngine;
using System.Collections.Generic;

public class AutoDespawn : MonoBehaviour
{
    private Dictionary<GameObject, float> objectsInTrigger = new Dictionary<GameObject, float>();
    public float disappearTime = 10f;

    void OnTriggerEnter(Collider other)
    {
        if (!objectsInTrigger.ContainsKey(other.gameObject))
        {
            objectsInTrigger.Add(other.gameObject, 0f);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (objectsInTrigger.ContainsKey(other.gameObject))
        {
            objectsInTrigger.Remove(other.gameObject);
        }
    }

    void Update()
    {
        // Make a copy of the keys to avoid modifying collection during iteration
        var keys = new List<GameObject>(objectsInTrigger.Keys);

        foreach (var obj in keys)
        {
            if (obj == null)
            {
                objectsInTrigger.Remove(obj);
                continue;
            }

            objectsInTrigger[obj] += Time.deltaTime;

            if (objectsInTrigger[obj] >= disappearTime)
            {
                Destroy(obj); // Or obj.SetActive(false);
                objectsInTrigger.Remove(obj);
            }
        }
    }
}
