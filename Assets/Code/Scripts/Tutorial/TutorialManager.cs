
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    public TextMeshProUGUI menuText;

    //Create a step class which has a condition, and then optionally has actions to perform at the start, and on the end of the step. Also has an end delay in case you dont want it to move on instantly
    public class TutorialStep
    {
        public string Message;
        public System.Func<bool> Condition;
        public System.Action OnStart;
        public System.Action OnEnd;
        public float EndDelay;

        private bool conditionMet = false;
        private float conditionTime = 0f;

        public TutorialStep(string message, System.Func<bool> condition, float endDelay = 0f, System.Action onStart = null, System.Action onEnd = null)
        {
            Message = message;
            Condition = condition;
            EndDelay = endDelay;
            OnStart = onStart;
            OnEnd = onEnd;
        }

        public bool IsComplete()
        {
            if (!conditionMet && Condition())
            {
                conditionMet = true;
                conditionTime = Time.time;
            }

            if (conditionMet)
            {
                return Time.time - conditionTime >= EndDelay;
            }

            return false;
        }
        public void End()
        {
            conditionMet = false;
            OnEnd?.Invoke();
        }
    }

    
    //Current step of progress through the tutorial
    public int progressIndex = 0;
    //List of all the tutorial steps

    private List<TutorialStep> steps;

    //All of the required fields to make the tutorial function

    public GameObject rightPainting;
    public GameObject leftPainting;

    public GameObject leftControllerSphere;
    public GameObject rightControllerSphere;

    public GameObject leftController;
    public GameObject rightController;
    public float controllerDistanceTreshold;
    [SerializeField] private InputActionReference leftTriggerPressAction;
    [SerializeField] private InputActionReference rightTriggerPressAction;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationProvider teleportationProvider;
    public GameObject leftExampleController;
    public GameObject rightExampleController;
    public GameObject leftExampleTrigger;
    public GameObject rightExampleTrigger;
    public GameObject leftExampleGrip;
    public GameObject rightExampleGrip;
    public GameObject rightExampleJoystick;
    public GameObject leftExampleJoystick;
    public GameObject basketBallEquipment;
    public GameObject basketBallHoop; 
    public GameObject basketBall;
    public GameObject bookShelf;
    public GameObject book;
    public GameObject paintballGun;
    public GameObject paintballGunTable;
    public Renderer pauseButtonrenderer;
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public GameObject nextButton;
    private bool hasTeleported = false;
    public UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationArea teleportationArea;
    private UnityEngine.Vector3 bookOrigin;
    private bool nextButtonPressed = false;
    private bool leftTriggerPressed = false;
    private bool rightTriggerPressed = false;
    public GameObject mainMenu;
    public GameObject platform;
    public AudioSource narrator;
    public AudioClip[] narratorDialogue;


    void Awake()
    {
        leftTriggerPressAction.action.Enable();
        rightTriggerPressAction.action.Enable();
    }

    void OnEnable()
    {
        teleportationProvider.locomotionEnded += OnPlayerTeleported;
    }

    void OnDisable()
    {
        teleportationProvider.locomotionEnded -= OnPlayerTeleported;
    }

    void Start()
    {        
        if (narratorDialogue.Length > 0 && narratorDialogue[0] != null)
        {
            narrator.PlayOneShot(narratorDialogue[0]);
        }

        //Create the tutorial steps
        steps = new List<TutorialStep>
        {
            new TutorialStep(
                "Hello and welcome to virtual reality! When you're ready, take a look at the painting on your right",
                () => rightPainting.GetComponent<Renderer>().isVisible,
                4,
                null,
                null
            ),

            
            new TutorialStep(
                "And now the painting on your left?",
                () => leftPainting.GetComponent<Renderer>().isVisible,
                2,
                null
            ),
              
            new TutorialStep(
                "Good! It seems you can still look around",
                () => true,
                3.5f,
                null,
                null
            ),


            new TutorialStep(
                "Next, put your controllers out infront of you",
                () => ControllersInFront(),
                0.5f,
                () => {
                    leftControllerSphere.SetActive(true);
                    rightControllerSphere.SetActive(true);
                },
                () => {
                    leftControllerSphere.SetActive(false);
                    rightControllerSphere.SetActive(false);
                }
            ),

            new TutorialStep(
                "Lets go through what these buttons do for you",
                () => true,
                3.5f,
                null,
                null
            ),

            new TutorialStep(
                "This is the 'trigger button' try pressing each one on your controllers",
                () => LeftTriggerPressed() && RightTriggerPressed(),
                0f,
                () => {
                    leftExampleController.SetActive(true);
                    rightExampleController.SetActive(true);
                    SetObjectHighlight(leftExampleTrigger, Color.cyan);
                    SetObjectHighlight(rightExampleTrigger, Color.cyan);
                    },

                () => {
                    leftExampleController.SetActive(false);
                    rightExampleController.SetActive(false);
                    SetObjectHighlight(leftExampleTrigger, Color.gray);
                    SetObjectHighlight(rightExampleTrigger, Color.gray);
                    leftTriggerPressed = false;
                    rightTriggerPressed = false;
                    }
            ),

            new TutorialStep(
                "Great job! Now point the controller at the next button and press the trigger to continue",
                () => CheckNextButtonPressed(),
                0f,
                () => {
                    nextButton.SetActive(true);
                },
                () => {
                    nextButtonPressed = false;
                    nextButton.SetActive(false);
                }
            ),

            new TutorialStep(
                "You're a natural. This is the 'grip button' and is used to pick things up. Try picking up this basketball",
                () => PickedUpBasketball(),
                0f,
                () => 
                {
                    leftExampleController.SetActive(true);
                    rightExampleController.SetActive(true);
                    basketBallEquipment.SetActive(true);
                    SetObjectHighlight(leftExampleGrip, Color.cyan);
                    SetObjectHighlight(rightExampleGrip, Color.cyan);
                },
                () => 
                {
                    leftExampleController.SetActive(false);
                    rightExampleController.SetActive(false);
                    SetObjectHighlight(leftExampleGrip, Color.gray);
                    SetObjectHighlight(rightExampleGrip, Color.gray);
                }
            ),
           
            new TutorialStep(
                "You've gotta know where this is going, try throwing the basketball in the hoop.",
                () => BallInHoop(),
                0f,
                () => {basketBallHoop.SetActive(true);
                basketBallEquipment.SetActive(true);},
                () => {
                    basketBallHoop.SetActive(false);
                    basketBallEquipment.SetActive(false);
                    basketBall.GetComponent<ResetBasketBall>().throughHoop = false;
                    }
            ),
             
            new TutorialStep(
                "Excellent, now try picking up this book and putting it on this shelf. You can just point your controller and press the button, no need to bend down",
                () => BookPlacedOnShelf(),
                2f,
                () => {
                    book.SetActive(true);
                    bookShelf.SetActive(true);
                    bookOrigin = book.transform.position;
                    },
                () => {
                    book.transform.position = bookOrigin;
                    book.SetActive(false);
                    bookShelf.SetActive(false);
                    }
            ),
            
            new TutorialStep(
                "Now, press this button to open the pause menu. You can bring this up at any time. Just resume to continue",
                () => pauseMenuActive(),
                0f,
                () => {SetObjectHighlight(pauseButton, Color.cyan);
                    leftExampleController.SetActive(true);
                    rightExampleController.SetActive(true);},
                () => {SetObjectHighlight(pauseButton, Color.gray);
                    leftExampleController.SetActive(false);
                    rightExampleController.SetActive(false);}
            ),

            new TutorialStep(
                "Now try combining the trigger and grip buttons to use this paintball gun! Grip it and then pull the trigger to fire!",
                () => UsedPaintGun(),
                0f,
                () => {paintballGun.SetActive(true); 
                paintballGunTable.SetActive(true);},
                () => {paintballGun.GetComponent<PaintGun>().numberOfPaintballsCreated = 0;}
            ),

            new TutorialStep(
                "Awesome, whenever you're ready we'll move onto movement. Push the right joystick forwards and aim where you would like to go",
                () => HasPlayerTeleported(),
                0f,
                () => {teleportationArea.enabled = true;
                SetObjectHighlight(rightExampleJoystick, Color.cyan);},
                () => {hasTeleported = false;
                SetObjectHighlight(rightExampleJoystick, Color.gray);}
            ),

            new TutorialStep(
                "Brilliant, whenever you're ready, move onto the red platform and we'll move onto how to select a module from the main menu",
                () => OnTriggerPlatform(),
                0f,
                () => {platform.SetActive(true);},
                () => {platform.SetActive(false);
                platform.GetComponent<OnPlatform>().entered = false;}
            ),
            
            new TutorialStep(
                "At the main menu, just use the trigger to select the module you would like. You can now replay the tutorial or move onto a module at your own pace.",
                () => false,
                0f,
                () => {mainMenu.SetActive(true);},
                () => {mainMenu.SetActive(false);}
            )
        };

        ShowMessage(steps[0].Message);
        steps[0].OnStart?.Invoke();
    }

    //All of the various conditions required by the tutorial steps
    bool ControllersInFront()
    {
        if (UnityEngine.Vector3.Distance(leftController.transform.position, leftControllerSphere.transform.position) < controllerDistanceTreshold && UnityEngine.Vector3.Distance(rightController.transform.position, rightControllerSphere.transform.position) < controllerDistanceTreshold)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool OnTriggerPlatform()
    {
        if (platform.GetComponent<OnPlatform>().entered == true)
        return true;
        else return false;
    }
    bool UsedPaintGun()
    {
        if (paintballGun.GetComponent<PaintGun>().numberOfPaintballsCreated > 0)
        {return true;}
        else 
        {return false;}
    }
    bool CheckNextButtonPressed()
    {
        return nextButtonPressed;
    }
    void SetNextButton(bool state)
    {
        nextButton.SetActive(state);
    }
    bool BookPlacedOnShelf()
    {
        if (book.GetComponent<Rigidbody>().linearVelocity.magnitude < 0.05f && book.GetComponent<BookInShelf>().inShelf && !book.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().isSelected)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    bool pauseMenuActive()
    {
        return pauseMenu.activeInHierarchy;
    }
    private void OnPlayerTeleported(LocomotionProvider provider)
    {
        hasTeleported = true;
    }
    private bool HasPlayerTeleported()
    {
        return hasTeleported;
    }
    void highlightPauseButton()
    {
        if (pauseButtonrenderer.material.color != Color.cyan)
        {
            pauseButtonrenderer.material.color = Color.cyan;
        }
        else
        {
            pauseButtonrenderer.material.color = Color.gray;
        }
    }
    void EnableBookEquipment()
    {
        book.SetActive(true);
        bookShelf.SetActive(true);
    }
    bool BallInHoop()
    {
        return basketBall.GetComponent<ResetBasketBall>().throughHoop;
    }
    public void NextButtonPressed()
    {
        nextButtonPressed = true;
        nextButton.SetActive(false);
    }
    public void BackButtonPressed()
    {
        
        // If we're past the last step (finished state)
        if (progressIndex >= steps.Count)
        {
            progressIndex = steps.Count - 1;

            steps[progressIndex].OnStart?.Invoke();
            ShowMessage(steps[progressIndex].Message);
            return;
        }

        // Normal back navigation
        if (progressIndex <= 0) return;

        steps[progressIndex].End();
        progressIndex--;

        steps[progressIndex].OnStart?.Invoke();

        if (progressIndex < narratorDialogue.Length && narratorDialogue[progressIndex] != null)
        {
            narrator.Stop();
            narrator.clip = narratorDialogue[progressIndex];
            narrator.Play();
        }

        ShowMessage(steps[progressIndex].Message);
    }

    void ShowMessage(string message)
    {
        menuText.text = message;
    }

    void SetControllerSpheres(bool state)
    {
        leftControllerSphere.SetActive(state);
        rightControllerSphere.SetActive(state);
    }
    void SetObjectHighlight(GameObject gameObject, Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = color;
    }
    bool PickedUpBasketball()
    {
        if (basketBall.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().isSelected) return true;
        else return false;
    }

    bool LeftTriggerPressed()
    {
         
        if (leftTriggerPressAction.action.triggered)
        {
            leftTriggerPressed = true;
        }
        return leftTriggerPressed;
    }

    bool RightTriggerPressed()
    {
        if (rightTriggerPressAction.action.triggered)
        {
            rightTriggerPressed = true;
        }
        return rightTriggerPressed;
    }

    void Update()
    {
        if (progressIndex >= steps.Count) return;

        if (steps[progressIndex].IsComplete())
        {
            steps[progressIndex].End();
            progressIndex++;

            if (progressIndex < steps.Count)
            {
                steps[progressIndex].OnStart?.Invoke();
                if (narratorDialogue[progressIndex] != null)
                {
                    narrator.Stop();
                    narrator.clip = narratorDialogue[progressIndex];
                    narrator.Play();
                }
                ShowMessage(steps[progressIndex].Message);
            }
        }
    }
}
