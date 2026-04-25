using UnityEngine;

public class PaintballExpire : MonoBehaviour
{
    //This script is really cursed but its working, the particle system was REFUSING to cooperate
    //Creates a seperate object on impact with anything that manages the particles
    public ParticleSystem impactParticles;
    public AudioClip impactSound;

    private AudioSource audioSource;
    private bool hasCollided = false;
    public GameObject paintballParticles;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hit: " + collision.gameObject.name);
        if (hasCollided) return;
        hasCollided = true;

        Instantiate(paintballParticles,this.transform);
        // Play sound
        if (impactSound != null)
        {
            AudioSource.PlayClipAtPoint(impactSound, collision.contacts[0].point);
        }

        // Spawn particles
        if (impactParticles != null)
        {
            ParticleSystem ps = Instantiate(
                impactParticles,
                collision.contacts[0].point,
                Quaternion.LookRotation(collision.contacts[0].normal)
            );

            ps.Play();
            Destroy(ps.gameObject, ps.main.duration);
        }

        Destroy(gameObject);
    }
}