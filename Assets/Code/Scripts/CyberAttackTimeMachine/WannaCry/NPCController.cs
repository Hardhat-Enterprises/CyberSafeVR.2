using System.Collections.Generic;
using UnityEngine;

namespace CATM_WC{
    public class CW_NPCController : MonoBehaviour
    {

        [System.Serializable]
        public class NPCData
        {
            public GameObject npc;

            [HideInInspector] public CW_NarrationManager narrationManager;
            [HideInInspector] public Transform pastPosition;
            [HideInInspector] public Transform futurePosition;
        }

        [Header("NPCs")]
        public List<NPCData> npcDataList = new List<NPCData>();

        public GameObject goodConsequence;

        void Awake()
        {
            foreach (var data in npcDataList)
            {
                if (data.npc == null) continue;

                // Find NarrationManager
                data.narrationManager = data.npc.GetComponentInChildren<CW_NarrationManager>();
                if (data.narrationManager == null)
                    Debug.LogWarning($"NarrationManager not found on {data.npc.name}");

                // Find position transforms as siblings of the NPC (sharing the same parent)
                Transform npcParent = data.npc.transform.parent;
                
                // Find sibling markers
                data.pastPosition = FindSiblingByName(data.npc.transform, "PastLocation");
                data.futurePosition = FindSiblingByName(data.npc.transform, "FutureLocation");

                
                if (data.pastPosition == null || data.futurePosition == null)
                    Debug.LogWarning($"One or more position markers missing as siblings of {data.npc.name}");
            }
        }

        public void DisasterPrevented()
        {
            goodConsequence.SetActive(true);
        }

        public void SendToPast()
        {
            foreach (var data in npcDataList)
            {
                if (data.narrationManager != null)
                    data.narrationManager.SetTime(0);

                MoveToPosition(data.npc, data.pastPosition);
            }
        }

        public void SendToFuture()
        {
            foreach (var data in npcDataList)
            {
                if (data.narrationManager != null)
                    data.narrationManager.SetTime(1);

                MoveToPosition(data.npc, data.futurePosition);
            }
        }

        private void MoveToPosition(GameObject npc, Transform target)
        {
            if (npc != null && target != null)
            {
                // Set the world position and rotation of the NPC to match the target's world transform
                npc.transform.position = target.position;
                npc.transform.rotation = target.rotation;
            }
            else
            {
                string npcName = npc != null ? npc.name : "null";
                string targetName = target != null ? target.name : "null";
                Debug.LogWarning($"Cannot move NPC: {npcName} to target: {targetName}");
            }
        }

        private Transform FindSiblingByName(Transform transform, string name)
        {
            if (transform == null || transform.parent == null) return null;
            
            Transform parent = transform.parent;
            
            // Look through all children of the parent (siblings of the transform)
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform sibling = parent.GetChild(i);
                if (sibling.name == name)
                {
                    return sibling;
                }
            }
            
            Debug.LogWarning($"Could not find sibling named '{name}' for {transform.name}");
            return null;
        }
    }
}