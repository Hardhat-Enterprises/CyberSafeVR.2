using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Events;

public class GuideNPC : MonoBehaviour
{
    public Transform playerTarget;
    public Transform exitTarget;
    public Animator animator;
    public UnityEvent onReachedPlayer;

    private NavMeshAgent agent;
    private bool isWalking = false;
    private AudioSource audioSource;  // Reference to AudioSource
    public AudioClip walkingSound;    // Walking sound clip

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource attached to NPC.");
        }
    }

    void Update()
    {
        // Update animator for speed (to play animation)
        animator.SetFloat("Speed", agent.velocity.magnitude);

        // If the agent is moving, make sure the sound is playing
        if (agent.velocity.magnitude > 0.1f && !audioSource.isPlaying)
        {
            PlayWalkingSound();  // Play walking sound if the agent starts moving
        }
        // If the agent stops, stop the sound
        else if (agent.velocity.magnitude <= 0.1f && audioSource.isPlaying)
        {
            StopWalkingSound();  // Stop walking sound if the agent stops
        }
    }

    public void GoToPlayer()
    {
        if (!isWalking)
        {
            StartCoroutine(WalkToPlayerRoutine());
        }
    }

    IEnumerator WalkToPlayerRoutine()
    {
        isWalking = true;
        agent.stoppingDistance = 1.5f;
        agent.SetDestination(playerTarget.position);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        agent.ResetPath();
        yield return StartCoroutine(FaceTarget(playerTarget));
        isWalking = false;
        onReachedPlayer.Invoke();

        StopWalkingSound();  // Stop walking sound once NPC reaches the player
    }

    public void WalkAway()
    {
        if (!isWalking && exitTarget != null)
        {
            StartCoroutine(WalkAwayRoutine());
        }
    }

    IEnumerator WalkAwayRoutine()
    {
        isWalking = true;
        agent.stoppingDistance = 0f;
        agent.SetDestination(exitTarget.position);

        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        agent.ResetPath();
        yield return StartCoroutine(FaceDirection(exitTarget.forward));
        isWalking = false;

        StopWalkingSound();  // Stop walking sound once NPC finishes walking away
    }

    IEnumerator FaceTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
            yield break;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        yield return StartCoroutine(RotateTo(targetRotation, 0.5f));
    }

    IEnumerator FaceDirection(Vector3 direction)
    {
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
            yield break;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        yield return StartCoroutine(RotateTo(targetRotation, 0.5f));
    }

    IEnumerator RotateTo(Quaternion targetRotation, float angleThreshold)
    {
        float rotationSpeed = 3f;

        while (Quaternion.Angle(transform.rotation, targetRotation) > angleThreshold)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    // Method to play the walking sound
    void PlayWalkingSound()
    {
        if (audioSource != null && walkingSound != null)
        {
            audioSource.loop = true;  // Loop the walking sound while walking
            audioSource.clip = walkingSound;
            audioSource.Play();
        }
    }

    // Method to stop the walking sound
    void StopWalkingSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
