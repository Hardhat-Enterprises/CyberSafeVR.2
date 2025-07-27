using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CloudLogEntry
{
    public string timestamp;
    public string user;
    public string eventName;
    public string ip;
}

public class CloudLogReader : MonoBehaviour
{
    public string fileName = "cloud_access_logs.json";
    public List<CloudLogEntry> logs;

    void Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            logs = new List<CloudLogEntry>(JsonHelper.FromJson<CloudLogEntry>(json));
            Debug.Log($"Loaded {logs.Count} log entries.");
        }
        else
        {
            Debug.LogError("Log file not found at: " + path);
        }
    }
}

// Helper for parsing arrays from JSON
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string wrappedJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrappedJson);
        return wrapper.Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
