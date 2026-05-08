using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.InputSystem;
//Manages the paintball gun
public class PaintGun : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    public GameObject paintBallPrefab;
    public float shootingPower = 10f;

    public float fireRate = 0.2f; // seconds between shots
    private float cooldownTimer = 0f;

    public int numberOfPaintballsCreated = 0;
    public GameObject muzzle;

    [SerializeField] private InputActionReference leftTriggerPressAction;
    [SerializeField] private InputActionReference rightTriggerPressAction;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    void Update()
    {
        // reduce cooldown over time
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        bool triggerPressed =
            leftTriggerPressAction.action.ReadValue<float>() > 0.5f ||
            rightTriggerPressAction.action.ReadValue<float>() > 0.5f;

        if (grabInteractable.isSelected && cooldownTimer <= 0f && triggerPressed)
        {
            Shoot();
            cooldownTimer = fireRate;
        }
    }

    void Shoot()
    {
        GameObject paintBall = Instantiate(paintBallPrefab, muzzle.transform.position, muzzle.transform.rotation);

        Physics.IgnoreCollision(
            paintBall.GetComponent<Collider>(),
            GetComponent<Collider>());

        paintBall.GetComponent<Rigidbody>().linearVelocity =
            transform.forward * shootingPower;

        numberOfPaintballsCreated++;

        audioSource.Play();
    }
}