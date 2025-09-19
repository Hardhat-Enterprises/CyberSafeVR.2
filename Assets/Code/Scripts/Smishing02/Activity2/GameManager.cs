using System.Collections.Generic;
using UnityEngine;

namespace SMS02
{
    public class GameManager : MonoBehaviour
    {
        public GameObject smsPrefab;               // Your SMSMessage prefab
        public Transform screenAnchor;             // The PhoneScreen transform
        public List<string> messages;              // Text of each message
        public List<bool> isSmishingFlags;         // Whether each message is smishing

        void Start()
        {
            SpawnAll();
        }

        void SpawnAll()
        {
            for (int i = 0; i < messages.Count; i++)
            {
                GameObject go = Instantiate(smsPrefab, screenAnchor);
                go.transform.localPosition = Vector3.zero;
                SMSMessage sms = go.GetComponent<SMSMessage>();
                sms.messageText.text = messages[i];
                sms.isSmishing = isSmishingFlags[i];
            }
        }
    }
}
