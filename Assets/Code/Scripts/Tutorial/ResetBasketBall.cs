using UnityEngine;

public class ResetBasketBall : MonoBehaviour
{
    private UnityEngine.Vector3 origin;
    public bool throughHoop = false;
    void Start()
    {
        origin = transform.position;
    }

    //Teleport the ball back to the origin if it hits the floor or if it makes a dunk
    void Update()
    {
        if (transform.position.y < -1.5)
        {
            transform.position = origin;
            GetComponent<Rigidbody>().linearVelocity= new Vector3(0,0,0);
            GetComponent<Rigidbody>().angularVelocity=new Vector3(0,0,0);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Hoop")
        {
            throughHoop = true;
            transform.position = origin;
            GetComponent<Rigidbody>().linearVelocity= new Vector3(0,0,0);
            GetComponent<Rigidbody>().angularVelocity=new Vector3(0,0,0);
        }
    }
}
