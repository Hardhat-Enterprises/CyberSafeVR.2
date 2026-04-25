using UnityEngine;

//Makes sure the player has enetered the platform
public class OnPlatform : MonoBehaviour
{
    public bool entered = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            entered = true;
        }
    }
}
