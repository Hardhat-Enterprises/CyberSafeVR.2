using UnityEngine;
using UnityEngine.UI;
  // Needed for XRSocketInteractor

namespace SWB01
{
    public class GroupManager : MonoBehaviour
    {
        public void SetActiveGroup(string targetGroupName)
        {
            // Iterate through all direct children of the canvas (the groups)
            foreach (Transform group in transform)
            {
                // Get all buttons in this group
                Button[] buttons = group.GetComponentsInChildren<Button>(true);
                
                // Enable buttons if group name matches, disable if not
                bool shouldEnable = (group.name == targetGroupName);
                
                foreach (Button button in buttons)
                {
                    button.interactable = shouldEnable;
                }
                
                // Find and activate/deactivate the socket
                UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socket = FindSocketInGroup(group);
                if (socket != null)
                {
                    socket.gameObject.SetActive(shouldEnable);
                }
            }
        }
        
        private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor FindSocketInGroup(Transform group)
        {
            // Look for a child with XRSocketInteractor component
            return group.GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>(true);
        }
    }
}
