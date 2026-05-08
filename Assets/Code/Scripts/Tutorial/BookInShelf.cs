using UnityEngine;

public class BookInShelf : MonoBehaviour
{
    //Makes sure the player has put a book into the shelf
    public bool inShelf = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Shelf")
        inShelf = true;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Shelf")
        inShelf = false;
    }
}
