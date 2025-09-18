# CATM_WC Scripts
**Namespace:** CATM_WC

## ActivateSocket.cs

**Purpose:**  
Allows toggling an XR Socket Interactor on or off during gameplay.

**Attach To:**  
Any GameObject that has an `XRSocketInteractor` component.

**Public Methods:**  
- `ToggleSocket()` – Enables or disables the socket component. Can be called via Unity Events or other scripts.

**How It Works:**  
- On `Awake()`, the script retrieves the `XRSocketInteractor` component from the same GameObject.  
- If the component is missing, a warning is logged.  
- Calling `ToggleSocket()` flips the `enabled` state of the socket, effectively activating or deactivating it.


## AutoDespawn.cs

**Purpose:**  
Automatically despawns (destroys) objects that remain within a trigger zone for longer than a specified duration. Designed to prevent objects from lingering indefinitely in a scene.

**Attach To:**  
Any GameObject with a collider set as a trigger.

**Public Variables / Inspector Settings:**  
- `disappearTime` (float) – Time in seconds an object must remain in the trigger before it is despawned. Default: 10 seconds.

**How It Works:**  
1. `OnTriggerEnter(Collider other)` – Adds objects entering the trigger to a tracking dictionary with a timer starting at 0.  
2. `OnTriggerExit(Collider other)` – Removes objects leaving the trigger from the tracking dictionary.  
3. `Update()` – Increments timers for all objects inside the trigger. Once a timer exceeds `disappearTime`, the object is destroyed and removed from tracking.

**Notes:**  
- Tracks objects individually, allowing multiple objects to despawn independently.  
- Safe against objects being destroyed outside the script; checks for null references.  
- Can be adapted to use `SetActive(false)` instead of `Destroy()` if temporary deactivation is preferred.  

## CableController.cs

**Purpose:**  
Controls a cable or wire between two points, visually represented with a `LineRenderer`. Updates dynamically as one end moves.

**Attach To:**  
Any GameObject with a `LineRenderer` component. Both `endA` and `endB` must be assigned in the Inspector.

**Public Variables / Inspector Settings:**  
- `endA` (Transform) – Stationary end of the cable.  
- `endB` (Transform) – Movable end of the cable.  
- `cableWidth` (float) – Width of the cable line. Default: 0.02.  
- `lineSegments` (int) – Number of segments for the LineRenderer. More segments = smoother line.

**How It Works:**  
1. `Start()`  
   - Initializes the `LineRenderer` with the specified width and segment count.  
   - Ensures both ends have `Rigidbody` components (`endA` is kinematic, `endB` is movable).  

2. `Update()`  
   - Interpolates positions along the line from `endA` to `endB` and updates each segment of the `LineRenderer`.  
   - Creates the visual effect of a cable stretching between the two points.

**Notes:**  
- For realistic cable physics, additional constraints or physics-based line simulation can be added.  
- Can be reused for electrical cables, ropes, or hoses in VR interactions.  

## CardLabel.cs

**Purpose:**  
Stores a label or description for a card in the scene. Useful for displaying contextual information when the card is interacted with.

**Attach To:**  
Any card GameObject in the scene.

**Public Variables / Inspector Settings:**  
- `impactText` (string) – Text representing the card’s impact, e.g., "Payroll Delayed".

**How It Works:**  
- This script does not contain runtime logic.  
- Its value (`impactText`) can be accessed by other scripts for display or game logic purposes.

## CW_NarrationManager.cs

**Purpose:**  
Handles context-sensitive narration triggered by the Guide NPC in the CATM_WC scene. Supports multiple timelines (past/future) and conditional scenarios (disaster/fixed).

**Attach To:**  
A GameObject containing an `AudioSource` (usually a narration manager object).

**Public Variables:**  
- **UI Elements**
  - `GameObject narrationCanvas` – Parent canvas for narration UI.
  - `GameObject narration` – Panel containing text for narration.
  - `TextMeshProUGUI narrationText` – Text element to display messages.
  - `GameObject narrationTrigger` – Trigger object to start narration.  
- **NPC Reference**
  - `GuideNPC guideNPC` – Reference to the NPC that rotates to face the player.  
- **Audio Clips**
  - `AudioClip textAppearClip` – Optional fallback for text narration.  
- **Timeline and Condition**
  - `int time` – Timeline (0 = past, 1 = future).  
  - `int cond` – Condition (0 = disaster, 1 = fixed).  
- **Narration Lines**
  - `string[] disasterMessages`, `AudioClip[] disasterAudioClips`  
  - `string[] savedMessages`, `AudioClip[] savedAudioClips`  
  - `string[] pastMessages`, `AudioClip[] pastAudioClips`  
- **Post-Narration**
  - `bool activateObjectAfterNarration` – If true, activates `objectToActivateAfterNarration` after narration completes.  
  - `GameObject objectToActivateAfterNarration` – Object to activate post-narration.

**Public Methods:**  
- `SetTime(int i)` – Sets the timeline.  
- `SetCond()` – Sets condition to fixed (1).  
- `NextMessage()` – Hides UI and triggers NPC to face the player, starting the narration sequence.  

**How It Works:**  
1. On `Awake()`, the script automatically finds the UI components and GuideNPC in the scene hierarchy.  
2. When `NextMessage()` is called, the narration trigger is disabled and the NPC rotates to face the player.  
3. Once the NPC faces the player (`OnNPCFacedPlayer`), the appropriate messages and audio play depending on `time` and `cond`.  
4. After all messages are played, the narration panel is hidden, the trigger is re-enabled, and any designated object can be activated.  
5. Uses an `AudioSource` to play per-message clips or a fallback clip.

## DamageablePart.cs

**Purpose:**  
Represents a part of a machine or object that can be "repaired" by interacting with it (e.g., using a hammer in VR). Tracks repair progress and triggers events when fully repaired.

**Attach To:**  
Any GameObject that represents a repairable component. The parent GameObject will be deactivated when repair completes.

**Public Variables:**  
- **Repair Settings**
  - `float repairTime` – Duration (in seconds) the part must be interacted with to complete repair.
  - `GameObject sparkEffect` – Particle effect (e.g., sparks) to indicate repair in progress.  
- **Events**
  - `UnityEvent onRepaired` – Event invoked when the repair is finished. Can be used to trigger sounds, animations, or game progression.

**Private Variables:**  
- `float currentRepairProgress` – Tracks accumulated repair time.

**Public Methods:**  
- `RepairStep(float deltaTime)` – Call this method repeatedly while the player is interacting with the part. Adds `deltaTime` to repair progress and completes the repair if enough time has passed.

**Behavior:**  
1. On `Start()`, the spark effect is activated (if assigned).  
2. Each time `RepairStep()` is called, repair progress accumulates.  
3. When progress meets or exceeds `repairTime`:
   - The part is considered repaired.
   - `onRepaired` is invoked.
   - The parent GameObject is deactivated to visually remove the part from the scene.  

**Usage Example:**  
```csharp
// In a hammer script:
void OnCollisionStay(Collision collision)
{
    DamageablePart part = collision.gameObject.GetComponent<DamageablePart>();
    if (part != null)
    {
        part.RepairStep(Time.deltaTime);
    }
}
```

## LidController.cs

**Purpose:**  
Controls the opening and closing of a trash lid when paper objects (tagged "RedLetter") are placed inside. Automatically destroys released paper objects and triggers optional events.

**Attach To:**  
The lid GameObject in the CyberAttack Time Machine (CATM) scene.

**Public Variables / Inspector Settings:**  
- `lidVisual` (Transform) – The visual mesh of the lid. If not assigned, defaults to the GameObject itself.  
- `openAngle` (float) – Angle (negative) by which the lid rotates downward when opening. Default: -45.  
- `lidOpenSpeed` (float) – Speed of lid opening rotation.  
- `lidCloseSpeed` (float) – Speed of lid closing rotation.  
- `onPaperDestroyed` (UnityEvent) – Invoked whenever a paper object is released and destroyed.

**Functionality / Usage:**  
1. Detects paper objects entering its trigger collider (`OnTriggerEnter`) and opens the lid.  
2. Tracks all `XRGrabInteractable` papers touching the lid.  
3. Automatically destroys papers when they are released (`CheckForReleasedPapers`).  
4. Closes the lid smoothly if no papers are in contact.  
5. Maintains lid rotation using `Quaternion.Slerp` for smooth animation.  
6. Optional event `onPaperDestroyed` can be used to trigger other game logic.

**Notes:**  
- The lid’s collider must be set as a trigger. The script sets this automatically if needed.  
- Papers must have the tag `RedLetter` and an `XRGrabInteractable` component.  
- The lid only opens when papers are detected and closes once all tracked papers are released.

## DoorController.cs

**Purpose:**  
Controls the opening and closing of a door with smooth rotation, scaling, and sound feedback. Supports locking to prevent opening.

**Attach To:**  
The door GameObject in the CyberAttack Time Machine (CATM) scene.

**Public Variables / Inspector Settings:**  
- `openAngle` (float) – Rotation angle applied when opening the door. Default: 90.  
- `rotationAxis` (Vector3) – Axis around which the door rotates. Default: Vector3.up.  
- `animationDuration` (float) – Time taken to animate door opening/closing.  
- `scaleFactor` (float) – Scale applied along the x-axis when the door opens. Default: 0.8.  
- `openClip` (AudioClip) – Sound played when door starts opening.  
- `closeClip` (AudioClip) – Sound played when door finishes closing.  
- `lockedClip` (AudioClip) – Sound played if an attempt is made to open a locked door.  
- `isLocked` (bool) – If true, door cannot be opened.

**Functionality / Usage:**  
1. `ToggleDoor()` – Opens or closes the door depending on its current state. Plays appropriate sound.  
2. Smoothly animates both rotation and scale over `animationDuration`.  
3. Prevents multiple simultaneous animations (`isAnimating`).  
4. Supports locking: if `isLocked` is true, attempts to open play `lockedClip`.  
5. `LockDoor(bool locked)` – Public method to lock or unlock the door.  

**Notes:**  
- The door rotates relative to its initial local rotation.  
- Scale is only affected on the x-axis when opening.  
- Attach an `AudioSource` to the door GameObject to play sounds.  
- Coroutine handles smooth animation and ensures the door ends in exact target rotation/scale.

## GameManager.cs

**Purpose:**  
Manages the timeline, NPC interactions, progress tracking, and disaster signs for the CyberAttack Time Machine (CATM) activity.

**Attach To:**  
The main GameManager GameObject in the CATM_WC scene.

**Public Variables / Inspector Settings:**  
- `timelineState` (int) – Tracks timeline: 0 = Past, 1 = Future.  
- `saved` (int) – Tracks number of fixes applied by the player.  
- `cards` (int, private) – Tracks how many cards have been fixed.  
- `npcController` (CW_NPCController) – Reference to the NPC controller.  
- `onDone` (UnityEvent) – Invoked when all three fixes are completed.  
- `onBackToFuture` (UnityEvent) – Invoked when switching to the future timeline.  
- `onBackToPast` (UnityEvent) – Invoked when switching to the past timeline.  
- `onCardsFixed` (UnityEvent) – Invoked when all three cards have been fixed.  
- `disasterSigns` (List<GameObject>) – Holds disaster sign objects that can be activated or deactivated.

**Functionality / Usage:**  
1. **Fix Methods** – `FixUpdate()`, `FixPhishing()`, `FixBackup()`  
   - Increment `saved` and call `TestConds()` to check if all fixes are completed.  
   - When `saved` reaches 3, `onDone` is invoked.

2. **Timeline Methods** – `Future()`, `Past()`  
   - `Future()` invokes `onBackToFuture`.  
   - `Past()` deactivates disaster signs and invokes `onBackToPast`.  

3. **Card Tracking** – `CheckCardForDoor()`  
   - Increments `cards` count.  
   - When `cards` reaches 3, invokes `onCardsFixed`.

4. **Timeline State Utility** – `SetTimelineState(int state)`  
   - Sets `timelineState` safely between 0 and 1.

5. **Disaster Signs Control** – `ActivateDisasterSigns()`, `DeactivateDisasterSigns()`  
   - Enable or disable all disaster signs assigned in the inspector.

**Notes:**  
- All public methods can be called from buttons, triggers, or other scripts.  
- Tracks player progress for both timeline actions and card-based objectives.  
- Provides hooks (UnityEvents) for triggering animations, sounds, or other game logic when milestones are reached.


## GuideNarrationManager.cs

**Purpose:**  
Manages narration for a scene, displaying text and playing audio clips sequentially. Designed for guide narration in the CATM_WC module.

**Attach To:**  
Any GameObject that should handle guide narration in a scene.

**Public Variables / Inspector Settings:**  
- `narrationCanvas` (GameObject) – Parent canvas containing the narration UI. Automatically assigned in `Awake()`.  
- `narration` (GameObject) – UI object displaying the narration text. Automatically assigned in `Awake()`.  
- `narrationText` (TextMeshProUGUI) – The text component to update with messages.  
- `narrationTrigger` (GameObject) – Trigger object controlling when narration starts or is visible. Automatically assigned in `Awake()`.  
- `sceneMessages` (string[]) – Array of text messages to display in sequence.  
- `sceneAudioClips` (AudioClip[]) – Array of audio clips corresponding to each message.  
- `textAppearClip` (AudioClip) – Optional fallback sound for text display.

**Functions / Example Usage:**  
1. `NextMessage()` – Plays the next message and audio in sequence. Hides the trigger while narration is playing and re-enables it when done.  
2. `AddMessage(string newMessage)` – Adds a new message to the existing `sceneMessages` array.  
3. `AddAudioClip(AudioClip newClip)` – Adds a new audio clip to the `sceneAudioClips` array.  
4. `ClearMessages()` – Resets `sceneMessages` and `sceneAudioClips` to empty arrays and resets index.  

**Notes:**  
- Automatically finds sibling objects for `narration` and `narrationTrigger` during `Awake()`.  
- Stops any currently playing audio before playing the next message.  
- If no audio clip is provided for a message, plays `textAppearClip` as a fallback.  
- Designed for sequential narration; does not loop messages unless called again manually.

## GuideNPC.cs

**Purpose:**  
Controls an NPC that can rotate to face the player and trigger events when aligned.

**Attach To:**  
Any NPC GameObject in the scene.

**Public Variables:**  
- `Transform playerTarget` – The target (usually the player) the NPC will face.  
- `UnityEvent onFacedPlayer` – Event invoked after the NPC finishes turning to face the target.

**Public Methods:**  
- `FacePlayer()` – Starts the rotation coroutine to face the specified target.

**How It Works:**  
- On `Start()`, the NPC’s initial rotation is stored.  
- `FacePlayer()` begins a coroutine that calculates the horizontal direction to the target, ignoring vertical difference.  
- `RotateTo()` gradually rotates the NPC toward the target over time, using a fixed rotation speed.  
- Once the NPC is aligned within a small angle threshold, `onFacedPlayer` is invoked.  
- Uses coroutines to smoothly animate the rotation rather than snapping instantly.

## LetterSpawner.cs

**Purpose:**  
Spawns letters (white and red) at set intervals during the CATM_WC activity.

**Attach To:**  
A GameObject in the scene designated as the letter spawn point.

**Public Variables / Inspector Settings:**  
- `whiteLetterPrefab` (GameObject) – Prefab for white letters.  
- `redLetterPrefab` (GameObject) – Prefab for red letters.  
- `whiteSpawnInterval` (float) – Time in seconds between spawning white letters. Default = 1.  
- `redSpawnInterval` (float) – Time in seconds between spawning red letters. Default = 2.  

**Functionality / Usage:**  
1. **Automatic Spawning** –  
   - White and red letters are spawned automatically in `Update()` based on their respective timers.  
   - After spawning, the timer for that letter type is reset.

2. **Spawn Method** – `SpawnLetter(GameObject letterPrefab)`  
   - Instantiates the given letter prefab at the spawner’s position and rotation.  
   - Checks if the prefab is assigned; logs a warning if not.

**Notes:**  
- Both white and red letters can be assigned different prefabs.  
- Spawn intervals can be adjusted in the inspector for gameplay balancing.  
- Spawner does not manage letter destruction; that must be handled elsewhere (e.g., `LidController` or floor collision scripts).

## LockTransform.cs

**Purpose:**  
Keeps the GameObject’s position and rotation locked to a target Transform.

**Attach To:**  
Any GameObject that should follow another object’s transform in the scene.

**Public Variables / Inspector Settings:**  
- `targetTransform` (Transform) – The transform that this object will follow and match.

**Functionality / Usage:**  
1. **Locking Position & Rotation** –  
   - In `LateUpdate()`, the script sets the GameObject’s position and rotation to match the `targetTransform`.  
   - Ensures the object follows the target smoothly each frame.

2. **Set Target Method** – `SetTarget(Transform newTarget)`  
   - Allows dynamically assigning a new target transform at runtime.  

**Notes:**  
- If `targetTransform` is `null`, the object remains stationary.  
- Useful for aligning objects, e.g., attaching papers to a lid or syncing UI elements with world objects.

## NarrationTrigger.cs

**Purpose:**  
Activates or deactivates a narration GameObject when the player enters or exits a trigger area.

**Attach To:**  
Any GameObject with a Collider set as a trigger, representing the area that will trigger narration.

**Public Variables / Inspector Settings:**  
- `narrationTrigger` (GameObject) – The GameObject containing the narration UI or logic to activate/deactivate.

**Functionality / Usage:**  
1. **Trigger Enter** –  
   - When an object tagged `"Player"` enters the trigger, `narrationTrigger` is set active.  
   - Typically used to show narration text or start audio narration.

2. **Trigger Exit** –  
   - When the player leaves the trigger, `narrationTrigger` is deactivated.  

**Notes:**  
- Ensure the collider is set as a trigger.  
- `narrationTrigger` should be assigned in the Inspector.  
- Tag the player object as `"Player"` for proper detection.

## CW_NPCController.cs

**Purpose:**  
Manages NPCs in the CyberAttack Time Machine scenario, including their positions in past/future timelines, narration state, and activating consequences when disasters are prevented.

**Attach To:**  
A parent GameObject that contains all NPCs as children (or otherwise references them in the `npcDataList`).

**Public Variables / Inspector Settings:**  
- `npcDataList` (List<NPCData>) – List of NPCs to manage. Each `NPCData` contains:
  - `npc` (GameObject) – The NPC GameObject.
  - `narrationManager` (CW_NarrationManager, auto-assigned) – Handles NPC narration.
  - `pastPosition` / `futurePosition` (Transform, auto-assigned) – Sibling markers for timeline positions.
- `goodConsequence` (GameObject) – Object to activate when a disaster is prevented.

**Public Methods:**  
1. `DisasterPrevented()` – Activates the `goodConsequence` object.  
2. `SendToPast()` – Moves all NPCs to their past positions and sets their narration to the past timeline.  
3. `SendToFuture()` – Moves all NPCs to their future positions and sets their narration to the future timeline.

**Internal Logic:**  
- Automatically searches for `CW_NarrationManager` in each NPC.  
- Searches for sibling markers named `"PastLocation"` and `"FutureLocation"` to define positions in timelines.  
- Logs warnings if an NPC or position marker is missing.  

**Notes / Usage Tips:**  
- Ensure each NPC GameObject has a `CW_NarrationManager` component in its hierarchy.  
- Create empty GameObjects as siblings of the NPC to serve as `"PastLocation"` and `"FutureLocation"` markers.  
- Assign `goodConsequence` to a visible object (e.g., UI element or effect) that shows when a disaster is prevented.  

## NPCGuide.cs

**Purpose:**  
Controls NPC movement and teleportation between predefined locations with visual effects and rolling animation. Supports multiple target locations and ensures the NPC follows a `LockTransform` target.

**Attach To:**  
NPC GameObject with an Animator component.

**Public Variables / Inspector Settings:**  
- `poofPrefab` (GameObject) – Visual effect prefab instantiated at the old and new location when teleporting.  
- `lockTransform` (LockTransform) – Component that ensures the NPC follows a target transform after teleportation.  
- `locations` (List<Transform>) – List of target positions the NPC can teleport to.

**Public Methods:**  
1. `GoToLocation(int index)` – Teleports the NPC to the target location at the specified index in `locations`. Automatically starts the roll animation and triggers poof effects.  
2. `startRoll()` – Sets the Animator parameter `"Roll_Anim"` to true.  
3. `stopRoll()` – Sets the Animator parameter `"Roll_Anim"` to false.

**Internal Logic:**  
- Uses a coroutine `TeleportRoutine` to handle teleportation timing, poof effects, and animation.  
- Prevents multiple teleportations at the same time (`isTeleporting` flag).  
- Validates index to avoid out-of-range errors.  
- Updates `currentLocation` to track where the NPC currently is.

**Notes / Usage Tips:**  
- Ensure the NPC has an Animator with a boolean parameter `"Roll_Anim"` for rolling animation.  
- Add all teleport target transforms to the `locations` list in the correct order.  
- Assign `lockTransform` to a LockTransform component on the NPC to follow the target properly.  
- `poofPrefab` is optional; leave null if visual effects are not needed.

## RepairHammer.cs

**Purpose:**  
Controls the motion of a hammer used for repairing objects and applies repair logic to `DamageablePart` components on collision.

**Attach To:**  
The hammer GameObject that will swing and interact with damageable objects.

**Public Variables / Inspector Settings:**  
- `swingSpeed` (float) – Speed at which the hammer swings back and forth.  
- `swingAngle` (float) – Maximum angle (degrees) of hammer swing.  

**Public Methods:**  
1. `StartHammering()` – Begins the swinging motion. Resets the swing timer.  
2. `StopHammering()` – Stops swinging and resets the hammer rotation to its initial state.

**Internal Logic:**  
- Uses `Mathf.Sin` to swing the hammer back and forth smoothly on the local Y-axis.  
- `isHammering` flag prevents movement when not in use.  
- `swingTimer` accumulates over time and is scaled by `swingSpeed`.  
- On collision (`OnTriggerStay`), checks for a `DamageablePart` component and applies repair via `RepairStep(Time.deltaTime)`.

**Usage Notes:**  
- The hammer should have a collider set to trigger for proper repair detection.  
- `DamageablePart` scripts on the objects define the repair behavior.  
- Adjust `swingSpeed` and `swingAngle` to fit the visual and gameplay feel.  
- Make sure the hammer’s pivot is correctly oriented for the desired swing motion.

## RespawnOnFloorHit.cs

**Purpose:**  
Automatically resets an object’s position and rotation when it touches a floor (or any specified collider) to prevent it from falling out of bounds.

**Attach To:**  
Any GameObject that can fall or be dropped (e.g., papers, props).

**Public Variables / Inspector Settings:**  
- `floorTag` (string) – Tag of the collider that triggers the respawn (default: "Floor").

**Internal Logic:**  
- Stores the original position and rotation of the object on `Start()`.  
- Uses `OnTriggerEnter` to detect collisions with objects tagged `floorTag`.  
- Calls `Respawn()` to reset the transform.  
- If a Rigidbody exists, also resets linear and angular velocity to zero, preventing lingering motion.

**Usage Notes:**  
- Ensure the floor collider has the correct tag.  
- The object must have either a `Collider` (trigger enabled) or Rigidbody setup to detect collisions correctly.  
- Useful for objects in VR or physics-based scenes to prevent them from getting lost or stuck.

## SpecificObjectSocket.cs

**Purpose:**  
Allows a socket to only accept objects that have a `SpiderController` component.

**Attach To:**  
Any XR Socket Interactor in your scene that should restrict accepted objects.

**Behavior:**  
- Overrides `CanSelect(IXRSelectInteractable interactable)` from `XRSocketInteractor`.  
- Checks if the object being placed has a `SpiderController` component.  
- If yes, the socket behaves normally (`base.CanSelect`).  
- If no, the object cannot be placed in this socket.

**Usage Notes:**  
- Useful for puzzles or mechanics where only specific objects can interact with a socket.  
- Works automatically with the XR Interaction Toolkit’s selection system.

## SpiderController.cs

**Purpose:**  
Controls the movement and animation of a spider NPC that moves back and forth between two points and can be placed idle.

**Attach To:**  
Spider GameObject with `Animator` and `XRGrabInteractable` components.

**Public Variables / Inspector Settings:**  
- `pointA` (Transform) – One end of the movement path.  
- `pointB` (Transform) – Other end of the movement path.  
- `moveSpeed` (float) – Speed at which the spider moves between points.  
- `waitAtPoint` (float) – Optional pause time when reaching a point.  

**Dependencies:**  
- Requires `Animator` for walking/idle animations.  
- Requires `Rigidbody` to be present (made kinematic automatically).  
- Uses `XRGrabInteractable` if object is grabbable in XR.

**Functions / Example Usage:**  
1. `Update()` – Automatically moves the spider toward the current target and plays the "Walk" animation.  
2. `FlipAtPoint()` – Coroutine that flips the spider 180° at each end point and optionally waits.  
3. `StartIdle()` – Stops movement and plays the "Idle" animation; can be called when the spider is placed in the scene.

**Notes:**  
- Spider movement stops if `placed` or `flipping` is true.  
- Rigidbody is automatically set to kinematic to avoid physics interference.  
- Ideal for VR/AR interactions where the spider can be grabbed or observed.

## TagSocketValidator.cs

**Purpose:**  
Validates objects placed into a socket based on a matching tag or expected label. Plays feedback sounds and locks correct objects in place.

**Attach To:**  
Any GameObject with an `XRSocketInteractor` that needs to validate incoming objects (e.g., cards).

**Public Variables / Inspector Settings:**  
- `GameManager` (GameManager) – Reference to the game manager for additional checks or progression logic.  
- `expectedImpactText` (string) – The expected text on the object to validate a correct placement.  
- `correctSound` (AudioClip) – Sound played when a correct object is placed.  
- `incorrectSound` (AudioClip) – Sound played when an incorrect object is placed.  
- `lockDelay` (float) – Delay before locking a correct object in place (seconds).

**Dependencies:**  
- Requires `UnityEngine.XR.Interaction.Toolkit`.  
- Requires an `AudioSource` on the same GameObject for sound playback.  
- Objects to validate should have a `CardLabel` component with `impactText`.

**Functions / Example Usage:**  
1. `ValidateMatch(SelectEnterEventArgs args)` – Checks if the object entering the socket matches the expected label or tag; triggers correct/incorrect feedback.  
2. `LockCardAfterDelay(GameObject card, float delay)` – Coroutine that disables grabbing and physics for a correct object after a delay.  
3. `PlaySound(AudioClip sound)` – Plays a given audio clip through the attached `AudioSource`.

**Notes:**  
- Incorrect objects are not rejected from the socket, but play an error sound.  
- Ensures correct objects are locked in place for further interactions.  
- Ideal for VR scenarios where players must place items in a precise order or location.

## TeleportNarration.cs

**Purpose:**  
Manages conditional narration lines for teleport events, choosing between fixed and not-fixed dialogue sequences and sending them to a `GuideNarrationManager`.

**Attach To:**  
Any GameObject responsible for triggering narration based on scene conditions.

**Public Variables / Inspector Settings:**  
- `narrationManager` (GuideNarrationManager) – Reference to the manager that handles displaying text and playing audio.  
- `cond` (int) – Condition flag (0 = not fixed, 1 = fixed) determining which set of messages to play.  
- `nfixedMessages` (string[]) – Lines to play when the condition is not fixed.  
- `nfixedAudioClips` (AudioClip[]) – Audio clips corresponding to `nfixedMessages`.  
- `fixedMessages` (string[]) – Lines to play when the condition is fixed.  
- `fixedAudioClips` (AudioClip[]) – Audio clips corresponding to `fixedMessages`.

**Functions / Example Usage:**  
1. `SetCond()` – Sets the condition flag to fixed (`cond = 1`).  
2. `ChooseMessages()` – Checks the `cond` flag and sends the appropriate messages and audio clips to the `narrationManager`.  
3. `SendMessages(string[] sceneMessages, AudioClip[] sceneAudioClips)` – Helper function that clears the narration manager, adds messages and audio clips, and triggers playback.

**Notes:**  
- Supports multiple lines and audio clips per condition.  
- Audio clips are optional; if fewer clips than messages exist, the remaining messages play without audio.  
- Designed for VR scenarios where narration varies depending on player actions or scene state.

## TriggerCanvasFollower.cs

**Purpose:**  
Makes a canvas or UI element follow a target (usually the VR camera) while maintaining a fixed horizontal distance and height relative to its parent pivot. Useful for NPCs or interactive elements that should face the player dynamically.

**Attach To:**  
Any GameObject containing the canvas or UI element that should follow the player.

**Public Variables / Inspector Settings:**  
- `target` (Transform) – The object to follow, typically the XR camera.  
- `orbitSpeed` (float) – Speed at which the canvas moves around the parent to follow the target.

**Internal Calculations:**  
- `horizontalDistance` – Maintains the original horizontal distance from the parent pivot.  
- `heightOffset` – Maintains the original vertical offset relative to the parent pivot.  
- `pivot` – Parent transform of the canvas, used as the reference point for movement.

**Behavior / Example Usage:**  
1. On `Start()`, calculates horizontal distance and height offset relative to the parent pivot.  
2. On `Update()`, moves the canvas smoothly toward the target while maintaining the calculated horizontal distance and height.  
3. Continuously rotates the canvas to face the target, ensuring UI is always readable.

**Notes:**  
- Requires the canvas to have a parent transform; otherwise, the script disables itself.  
- Only horizontal distance is maintained; vertical movement is based on a fixed offset.  
- Smoothly interpolates position using `Lerp` for fluid movement.
