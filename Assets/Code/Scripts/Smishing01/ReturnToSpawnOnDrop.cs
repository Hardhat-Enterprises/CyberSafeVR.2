using System.Collections;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class ReturnToSpawnOnDrop : MonoBehaviour
{
    [Header("Return behaviour")]
    public bool onlyIfNotInSocket = true;   // keep true: do NOT return if a socket grabs it
    public float returnDuration = 0.15f;    // smooth snap-back time (seconds)

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    Rigidbody rb;

    // we store LOCAL pose so it works even if parented under a spawner
    Transform originalParent;
    Vector3 spawnLocalPos;
    Quaternion spawnLocalRot;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        originalParent = transform.parent;
        spawnLocalPos = transform.localPosition;
        spawnLocalRot = transform.localRotation;

        // when player releases the card
        grab.selectExited.AddListener(_ => StartCoroutine(TryReturnAfterRelease()));
    }

    IEnumerator TryReturnAfterRelease()
    {
        // wait a frame so sockets (if any) can take selection
        yield return null;

        // if a socket is now selecting this, don't return (unless you turn the flag off)
        if (onlyIfNotInSocket && grab.interactorsSelecting.Any(i => i is UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor))
            yield break;

        // zero out physics & move smoothly back
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // ensure original parent (optional)
        transform.SetParent(originalParent, true);

        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, returnDuration);
            transform.localPosition = Vector3.Lerp(startPos, spawnLocalPos, t);
            transform.localRotation = Quaternion.Slerp(startRot, spawnLocalRot, t);
            yield return null;
        }

        transform.localPosition = spawnLocalPos;
        transform.localRotation = spawnLocalRot;
    }

    // call this if you want to redefine the spawn point at runtime
    public void SetCurrentAsSpawn()
    {
        originalParent = transform.parent;
        spawnLocalPos = transform.localPosition;
        spawnLocalRot = transform.localRotation;
    }
}
