# SWB01 Scripts
**Namespace:** SWB01

## AnswerHandler.cs
**Purpose:**  
Handles tracking and validating answers in the Safe Web Browsing (SWB01) activity. Manages individual answer booleans, total count of correct answers, and triggers the end-of-game events when all answers are correct.

**Attach To:**  
GameObject that also has the `GameManager` component in the SWB01 activity scene.

**Public Variables / Inspector Settings:**  
- `URL` (bool) – Tracks if the URL answer is correct  
- `Encrypt` (bool) – Tracks if encryption answer is correct  
- `Cert` (bool) – Tracks if certificate answer is correct  
- `Homepage` (bool) – Tracks if homepage answer is correct  
- `Login` (bool) – Tracks if login answer is correct  
- `Ad` (bool) – Tracks if ad answer is correct  
- `totalCount` (int) – Tracks the total number of correct answers  
- `askEndGame` (UnityEvent) – Invoked when `totalCount` reaches 6

**Dependencies:**  
- Requires a `GameManager` component on the same GameObject.  
- Uses `UnityEngine.Events.UnityEvent` to handle end-of-game logic.

**Functions / Example Usage:**  
1. **SetAnswer(string answerName, bool value)**  
   - Marks a specific answer as true or false.  
   - Example: `SetAnswer("URL", true);`  

2. **CheckAnswers()**  
   - Checks if all answers are correct.  
   - Calls `SetEnding(end)` and `FinishGame()` on `GameManager`.

3. **IncrementTotal(int amount = 1)**  
   - Increases `totalCount` by the given amount.  
   - Invokes `askEndGame` if `totalCount` reaches 6.

4. **DecrementTotal(int amount = 1)**  
   - Decreases `totalCount` by the given amount.  
   - Prevents `totalCount` from going below 0.

5. **ResetAnswers()**  
   - Resets all answer booleans and `totalCount` to 0.

**Notes:**  
- Designed for SWB01 module activity; other modules/activities should not reuse this script without modification.  
- Works with `GameManager` to control game progression.


## ButtonPressChecker.cs

**Purpose:**  
Detects when a UI button is pressed and executes a response. Logs a message when the assigned button is clicked. Useful for testing button interactions or triggering simple events.

**Attach To:**  
Any GameObject in the scene. Typically attached to a manager object or the button itself.

**Public Variables / Inspector Settings:**  
- `yourButton` (Button) – Assign the Button component from the scene in the Inspector.

**Dependencies:**  
- Requires the `UnityEngine.UI` namespace.  
- Requires a `Button` component assigned in the Inspector.

**Functions / Example Usage:**  
1. Assign a Button from the scene to the `yourButton` field in the Inspector.  
2. On scene start, the script automatically adds a listener to the button’s `onClick` event.  
3. When the button is pressed, `OnButtonClick()` is called, which logs: `"Button has been clicked!"`  

**Notes:**  
- Can be extended to call other functions or trigger game events instead of logging.  
- Each button requires its own `ButtonPressChecker` if different behavior is needed.

## ComputerManager.cs

**Purpose:**  
Manages computer groups in the Safe Web Browsing (SWB01) activity. Handles navigation between groups, updates UI descriptions, and toggles visibility of group-specific items.

**Attach To:**  
A central manager GameObject in the SWB01 scene that controls the computer group UI.

**Public Variables / Inspector Settings:**  
- `groupManager` (GroupManager) – Reference to a GroupManager that handles activating groups.  
- `descriptionText` (TMP_Text) – TextMeshPro UI element displaying the current group description.  
- `leftArrowButton` (Button) – Button to navigate to the previous group.  
- `rightArrowButton` (Button) – Button to navigate to the next group.  
- `groupNames` (string[]) – Names of each computer group.  
- `groupDescriptions` (string[]) – Descriptions corresponding to each group.  
- `groupItems` (GameObject[]) – Objects representing each group; only the current group is active.

**Dependencies:**  
- Requires `UnityEngine.UI` and `TMPro`.  
- Requires a `GroupManager` script/component to handle group activation.

**Functions / Example Usage:**  
1. Assign all UI references and arrays in the Inspector.  
2. On scene start, the script initializes the first group and sets up button listeners.  
3. `NextGroup()` – Switches to the next group cyclically and updates UI.  
4. `PreviousGroup()` – Switches to the previous group cyclically and updates UI.  
5. `UpdateGroupDisplay()` – Updates group buttons, description text, and item visibility based on the current index.  
6. `GetCurrentGroupName()` – Returns the name of the currently selected group.  
7. `GetCurrentGroupDescription()` – Returns the description of the currently selected group.

**Notes:**  
- Ensure all arrays (`groupNames`, `groupDescriptions`, `groupItems`) have the same length to avoid index errors.  
- Can be combined with other UI scripts to handle additional interactions for each group.

## EndingChecker.cs

**Purpose:**  
Checks whether the object placed in an XR socket has the correct tag. If the wrong object is placed, it triggers a spotlight indicator and notifies the `GameManager` to set the ending condition.

**Attach To:**  
GameObject with an `XRSocketInteractor` component in the SWB01 activity scene.

**Public Variables / Inspector Settings:**  
- `correctTag` (string) – Tag that the object must have to be considered correct. Default is `"Safe"`.  
- `gameManager` (GameManager) – Reference to the GameManager that handles ending logic.  
- `spotLight` (GameObject) – Optional spotlight to activate when an incorrect object is placed.

**Dependencies:**  
- Requires `UnityEngine.XR.Interaction.Toolkit` for `XRSocketInteractor`.  
- Uses `System.Linq` for `FirstOrDefault()`.  
- Requires a `GameManager` component to handle ending logic.

**Functions / Example Usage:**  
1. Assign the XR socket GameObject with this script attached.  
2. Assign the `gameManager` reference and optional `spotLight` in the Inspector.  
3. Call `CheckCurrentObject()` when you want to validate the object in the socket:  
   - Retrieves the first object in the socket  
   - Compares its tag with `correctTag`  
   - If incorrect, activates `spotLight` (if assigned) and sets the ending in `GameManager`.

**Notes:**  
- Only checks the first object in the socket; multiple objects are ignored.  
- Ensure that objects meant to be correct are tagged appropriately in the Unity Editor.  
- Typically called by other scripts or UI events in the activity to determine if the player completed the task correctly.

## GameManager.cs

**Purpose:**  
Handles the ending logic for the SWB01 activity. Tracks the ending type (good or bad) and invokes the appropriate UnityEvents for game completion.

**Attach To:**  
A central manager GameObject in the SWB01 activity scene. Other scripts (e.g., `AnswerHandler`, `EndingChecker`) can reference this to set or trigger the ending.

**Public Variables / Inspector Settings:**  
- `endingType` (bool) – Determines the type of ending: `false` = Bad, `true` = Good.  
- `onBadEnding` (UnityEvent) – Event triggered when a bad ending occurs.  
- `goodEndingEvents` (EndingEventGroup) – List of UnityEvents invoked when the ending is good.  
- `badEndingEvents` (EndingEventGroup) – List of UnityEvents invoked when the ending is bad.  
- `endingEvents` (EndingEventGroup) – List of UnityEvents invoked for any ending (runs regardless of type).

**Dependencies:**  
- Uses `UnityEngine.Events.UnityEvent`.  
- Can be referenced by other SWB01 scripts to trigger ending conditions.

**Functions / Example Usage:**  
1. `SetEnding(bool ending)` – Sets the ending type.  
   - Example: `gameManager.SetEnding(true);`  
2. `FinishGame()` – Invokes the corresponding events for good/bad endings and all ending events.  
3. `InvokeEventGroup(EndingEventGroup group)` – Internal helper function to invoke all events in a group.  

**Notes:**  
- Good and bad ending events are organized in `EndingEventGroup` for modular invocation.  
- `onBadEnding` provides a quick way to trigger additional logic specifically for bad endings.  
- Ensure all UnityEvents are assigned in the Inspector to avoid null references.

## GroupManager.cs

**Purpose:**  
Manages activation of computer groups in the SWB01 activity. Enables or disables UI buttons and XR sockets based on the currently selected group.

**Attach To:**  
Canvas or parent GameObject containing all group GameObjects as direct children.

**Public Variables / Inspector Settings:**  
- No public fields; the script operates on all child objects of the GameObject it is attached to.  

**Dependencies:**  
- Uses `UnityEngine.UI` for Button interactions.  
- Uses `UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor` for enabling/disabling XR sockets.

**Functions / Example Usage:**  
1. `SetActiveGroup(string targetGroupName)` – Activates the group whose name matches `targetGroupName`:  
   - Enables all buttons in the selected group and disables buttons in other groups.  
   - Activates the XRSocketInteractor in the selected group and deactivates sockets in other groups.  
   - Example: `groupManager.SetActiveGroup("GroupA");`  

2. `FindSocketInGroup(Transform group)` – Internal helper that returns the `XRSocketInteractor` in the given group, if one exists.

**Notes:**  
- The script assumes each group is a direct child of the parent GameObject and has buttons and optionally an XRSocketInteractor as children.  
- Make sure group names in the hierarchy match the names used when calling `SetActiveGroup()`.  
- Designed to work with the `ComputerManager` script to update the UI and XR sockets dynamically.

## HideTalkButton.cs

**Purpose:**  
Hides a specified GameObject at the start of the scene. Useful for UI buttons or elements that should initially be invisible.

**Attach To:**  
Any GameObject in the scene (typically a manager or the button itself).

**Public Variables / Inspector Settings:**  
- `objectToHide` (GameObject) – Assign the GameObject you want to hide when the scene starts.

**Dependencies:**  
- None beyond standard `UnityEngine`.

**Functions / Example Usage:**  
1. Assign the target GameObject to `objectToHide` in the Inspector.  
2. On scene start, `Start()` automatically sets the GameObject to inactive:  
   - `objectToHide.SetActive(false);`

**Notes:**  
- This script only hides the object once at the start; it does not manage reactivation.  
- Can be combined with other scripts to show the object later based on game logic.

## ItemActivationEvent.cs

**Purpose:**  
Triggers a UnityEvent automatically when the GameObject is enabled. Useful for activating items, UI elements, or other game logic when an object becomes active.

**Attach To:**  
Any GameObject in the SWB01 scene that should trigger an event upon activation.

**Public Variables / Inspector Settings:**  
- `onActivated` (UnityEvent) – Event that will be invoked when the GameObject is enabled.

**Dependencies:**  
- Uses `UnityEngine.Events.UnityEvent`.

**Functions / Example Usage:**  
1. Assign a UnityEvent to `onActivated` in the Inspector.  
2. When the GameObject is set active (enabled) during runtime, the assigned event(s) will automatically execute.  
   - Example: enabling a collectible item could trigger a sound effect or UI update.

**Notes:**  
- The script only triggers events when the GameObject becomes active.  
- Ensure the events are assigned in the Inspector to avoid null references.

## LockTransform.cs

**Purpose:**  
Locks a GameObject’s position and rotation in the scene, preventing movement or rotation during runtime. Allows optional teleportation to a new position and rotation while maintaining the lock.

**Attach To:**  
Any GameObject that should remain fixed in space during gameplay.

**Public Variables / Inspector Settings:**  
- `lockedPosition` (Vector3) – Position to lock the GameObject to.  
- `lockedRotation` (Vector3) – Rotation (Euler angles) to lock the GameObject to.

**Dependencies:**  
- Standard `UnityEngine` library; no additional components required.

**Functions / Example Usage:**  
1. `LateUpdate()` – Called every frame to enforce locked position and rotation.  
2. `TeleportTo(Vector3 newPosition, Vector3 newRotation)` – Updates the locked position and rotation to a new value:  
   - Example: `lockTransform.TeleportTo(new Vector3(0,1,0), new Vector3(0,90,0));`  

**Notes:**  
- Position and rotation are enforced every frame via `LateUpdate()`.  
- Useful for objects that should remain stationary but may need repositioning programmatically.  
- Ensure `TeleportTo()` is called before the first frame update if moving an object at scene start.

## NamedSocketInteractor.cs

**Purpose:**  
Custom XR socket that only accepts and hovers over objects with a specific GameObject name. Ensures that only designated objects can be placed in the socket.

**Attach To:**  
Any GameObject with an `XRSocketInteractor` component where object name-based filtering is required.

**Public Variables / Inspector Settings:**  
- `allowedName` (string) – The exact name of the GameObject this socket will accept.

**Dependencies:**  
- Requires `UnityEngine.XR.Interaction.Toolkit`.  
- Extends `XRSocketInteractor`.

**Functions / Example Usage:**  
1. `CanSelect(IXRSelectInteractable interactable)` – Overrides the base selection logic to allow selection only if the interactable’s GameObject name matches `allowedName`.  
2. `CanHover(IXRHoverInteractable interactable)` – Overrides the base hover logic to allow hovering only if the interactable’s GameObject name matches `allowedName`.  
   - Example:  
     - Set `allowedName = "SafeFile";`  
     - Only GameObjects named `"SafeFile"` can be placed or hovered over in this socket.

**Notes:**  
- Useful in SWB01 for activities that require precise object placement.  
- Ensure GameObject names match exactly, including capitalization.  
- Works seamlessly with other XRSocketInteractor functionality.

## NarrationManager.cs

**Purpose:**  
Manages scene narration in the SWB01 activity by displaying text messages and playing corresponding audio clips. Handles sequential messages, UI visibility, and optional sound effects.

**Attach To:**  
A central manager GameObject in the scene with an `AudioSource` component.

**Public Variables / Inspector Settings:**  
- `narrationCanvas` (GameObject) – The canvas containing all narration UI elements.  
- `narration` (GameObject) – UI panel or object that displays the narration text.  
- `narrationText` (TextMeshProUGUI) – TextMeshPro element to show messages.  
- `interactButton` (GameObject) – Button for progressing or interacting with the scene.  
- `sceneMessages` (string[]) – Array of text messages to display sequentially.  
- `sceneAudioClips` (AudioClip[]) – Optional audio clips corresponding to each message.  
- `textAppearClip` (AudioClip) – Fallback audio clip if no specific clip is assigned.  
- `currentIndex` (int) – Tracks which message is currently active.

**Dependencies:**  
- Requires `TMPro` for text display.  
- Requires an `AudioSource` component to play audio clips.

**Functions / Example Usage:**  
1. `NextMessage()` – Displays the next text message and plays the corresponding audio:  
   - Hides the `interactButton` while the message is shown.  
   - Increments `currentIndex` and handles end-of-sequence logic.  
   - Invokes `OnNarrationComplete` event when all messages are finished.  
2. `AddMessage(string newMessage)` – Adds a new text message to the sequence.  
3. `AddAudioClip(AudioClip newClip)` – Adds a new audio clip to the sequence.  
4. `ClearMessages()` – Resets all messages and audio clips.  
5. `PlaySound(AudioClip clip)` – Plays a one-shot audio clip if assigned.

**Notes:**  
- Ensure `sceneMessages` and `sceneAudioClips` arrays align if audio should match text.  
- The script automatically handles UI visibility and interaction states.  
- `OnNarrationComplete` can be used to trigger next steps in the activity workflow.

## NPCGuide.cs

**Purpose:**  
Controls a non-player character (NPC) guide that can animate, teleport between predefined locations, and create visual effects during teleportation.

**Attach To:**  
NPC GameObject with an `Animator` component and a `LockTransform` script attached.

**Public Variables / Inspector Settings:**  
- `poofPrefab` (GameObject) – Prefab instantiated at the NPC’s position during teleportation for visual effect.  
- `lockTransform` (LockTransform) – Reference to the `LockTransform` component that maintains the NPC’s position and rotation.  
- `locationA` (Transform) – First teleport target location.  
- `locationB` (Transform) – Second teleport target location.

**Dependencies:**  
- Requires `Animator` for controlling roll animations.  
- Requires `LockTransform` for enforcing position/rotation after teleportation.  
- Uses `Coroutine` for timed teleportation with effects.

**Functions / Example Usage:**  
1. `GoToLocationA()` – Teleports NPC to `locationA` using a visual effect and roll animation.  
2. `GoToLocationB()` – Teleports NPC to `locationB` using a visual effect and roll animation.  
3. `startRoll()` / `stopRoll()` – Start and stop the rolling animation.  
4. `TeleportRoutine(Transform target)` – Internal coroutine handling teleportation, animation, and effects.

**Notes:**  
- NPC cannot teleport while a previous teleport is in progress (`isTeleporting` flag).  
- `poofPrefab` is optional; if not assigned, only the teleport occurs without effects.  
- Ensure `LockTransform` is assigned to maintain proper NPC positioning during teleport.  
- Can be triggered by player interaction or activity events in the scene.

## ObjectDropSound.cs

**Purpose:**  
Plays a sound effect when the GameObject collides with another object, typically used to indicate the object hitting the ground.

**Attach To:**  
Any GameObject that should emit a drop sound when colliding with other objects.

**Public Variables / Inspector Settings:**  
- `dropSound` (AudioClip) – The sound effect to play on collision.

**Dependencies:**  
- Requires an `AudioSource` component. If one is not present, the script will automatically add it.

**Functions / Example Usage:**  
1. `OnCollisionEnter(Collision collision)` – Called automatically by Unity when the GameObject collides with another collider.  
   - Plays `dropSound` once at a volume of 0.7.  
   - Example: Attach to a throwable object so it makes a sound when it hits the floor.

**Notes:**  
- Ensure the GameObject has a `Collider` component set to interact with physics.  
- Volume and pitch can be adjusted in the `AudioSource` settings if needed.  
- `dropSound` is optional; no sound will play if not assigned.

## RespawnOnFloorHit.cs

**Purpose:**  
Automatically respawns a GameObject to its original position and rotation when it collides with a floor (or any designated trigger). Useful for objects that can fall out of the play area.

**Attach To:**  
Any GameObject that needs to reset its position after hitting a floor or boundary.

**Public Variables / Inspector Settings:**  
- `floorTag` (string) – Tag of the floor or trigger collider that will trigger the respawn. Default is `"Floor"`.

**Dependencies:**  
- Requires a `Collider` component with `isTrigger` enabled on the floor object.  
- Optional `Rigidbody` on the object to reset velocity on respawn.

**Functions / Example Usage:**  
1. `OnTriggerEnter(Collider other)` – Detects collision with objects tagged as `floorTag` and triggers respawn.  
2. `Respawn()` – Resets the GameObject’s position, rotation, parent, and Rigidbody velocity:  
   - Example: An object falling off a table will automatically return to its starting position.  

**Notes:**  
- Stores the original parent to ensure the object hierarchy is maintained.  
- Works for both physics and non-physics objects.  
- Ensure `floorTag` matches the tag on the floor collider.

## SignLabel.cs

**Purpose:**  
Stores a simple label or identifier for a GameObject. Useful for signs, markers, or any object that requires a text identifier in the scene.

**Attach To:**  
Any GameObject that represents a sign, marker, or labeled object in the scene.

**Public Variables / Inspector Settings:**  
- `Label` (string) – The text or identifier associated with the object.

**Dependencies:**  
- None beyond `UnityEngine`.

**Functions / Example Usage:**  
- Can be used by other scripts to read the label at runtime:  
  - Example: `string signText = signGameObject.GetComponent<SignLabel>().Label;`  

**Notes:**  
- Does not include any UI or display functionality; purely a data container.  
- Useful in conjunction with scripts that check labels, such as XR interactions or educational modules.

## SocketConditionActivator.cs

**Purpose:**  
Monitors a set of XR sockets and triggers events once all assigned sockets are filled. Ensures events only fire once per complete fill, and resets if an item is removed.

**Attach To:**  
Any GameObject in the scene responsible for managing a set of `XRSocketInteractor` objects.

**Public Variables / Inspector Settings:**  
- `sockets` (XRSocketInteractor[]) – Array of sockets to monitor. Assign in the Inspector.  
- `onAllSocketsFilled` (UnityEvent) – Event(s) to invoke once all sockets are filled.  
- `gameManager` (GameManager) – Optional reference for additional game state handling.

**Dependencies:**  
- Requires `UnityEngine.XR.Interaction.Toolkit`.  
- Works with `XRSocketInteractor` objects for detecting selections and removals.

**Functions / Example Usage:**  
1. `OnItemPlacedInSocket(SelectEnterEventArgs args)` – Called automatically when an item is placed into a socket; triggers a check.  
2. `OnItemRemovedFromSocket(SelectExitEventArgs args)` – Called when an item is removed; resets the triggered state.  
3. `CheckSockets()` – Checks if all assigned sockets have a selection; invokes `onAllSocketsFilled` if true.  
4. `Reset()` – Resets the `hasTriggered` flag manually.

**Notes:**  
- Only triggers events once per complete fill to prevent repeated firing.  
- Automatically subscribes to socket events on `Start()` and unsubscribes on `OnDestroy()`.  
- Useful for SWB01 activities where multiple objects must be placed correctly to progress.

## SocketLabelHandler.cs

**Purpose:**  
Handles objects placed in a socket and updates the corresponding `AnswerHandler` boolean. It can distinguish between "Safe" and "Not Safe" objects, eject invalid objects, and manage the socket state.

**Attach To:**  
Any GameObject containing an `XRSocketInteractor` that represents a socket in the SWB01 module.

**Public Variables / Inspector Settings:**  
- `socket` (XRSocketInteractor) – The socket this handler manages.  
- `answerName` (string) – The name of the boolean in `AnswerHandler` (e.g., "URL", "Encrypt").  
- `ejectForce` (float) – Upward force applied when an invalid object is ejected.  
- `answerHandler` (AnswerHandler) – Reference to the `AnswerHandler` managing all answers.  

**Dependencies:**  
- Requires `AnswerHandler` to update the state of answers.  
- Requires `RespawnOnFloorHit` on objects for proper respawn functionality.  
- Works with `XRSocketInteractor` from `UnityEngine.XR.Interaction.Toolkit`.  

**Functions / Example Usage:**  
1. `OnObjectPlaced(SelectEnterEventArgs args)` – Called when an object is placed in the socket.  
   - Checks if the object is "Safe" or "Not Safe" based on its name.  
   - Updates the `AnswerHandler` and increments the total count if applicable.  
   - Deactivates the placed object and disables the socket.  

2. `OnObjectRemoved()` – Called when an object is removed or reset.  
   - Decrements the total count in `AnswerHandler`.  
   - Reactivates and respawns the last placed object if applicable.  

3. `EjectObject()` – Forces the currently placed object out of the socket if it’s invalid.  

**Notes:**  
- Naming conventions are critical: the object names must match `answerName + "Safe"` or `answerName + "NotSafe"`.  
- Ensures modularity by connecting socket interactions to the main `AnswerHandler`.  
- Designed for use in SWB01 activities where correct object placement is required to progress.

## HighlightActivator.cs

**Purpose:**  
Activates a highlight GameObject if a socket item is determined to be unsafe, typically after a “bad ending” event is triggered.  

**Attach To:**  
Any GameObject that has a `SocketLabelHandler` and is part of a SWB01 socket group.

**Public Variables / Inspector Settings:**  
- `gameManager` (GameManager) – Reference to the `GameManager` to subscribe to the `onBadEnding` event.  

**Dependencies:**  
- Requires `SocketLabelHandler` on the same GameObject.  
- Requires a sibling GameObject named `Highlight` under the same parent for activation.  
- Requires `GameManager` to handle bad ending events.

**Functions / Example Usage:**  
1. `Start()` – Subscribes to the `GameManager.onBadEnding` event and locates the sibling highlight GameObject.  
2. `CheckHighlight()` – Checks if the item in the socket is unsafe (`isSafe == false`) and activates the sibling highlight.  
3. `OnDestroy()` – Unsubscribes from the event to prevent memory leaks.

**Notes:**  
- The script assumes a sibling GameObject named `Highlight` exists.  
- Only activates if the `SocketLabelHandler` marks the item as unsafe.  
- Useful for visually guiding the user to incorrectly placed items after an activity ends.

## TemporaryNarration.cs

**Purpose:**  
Plays a temporary set of narration messages and audio clips, then restores the original narration once finished. Useful for situational prompts without permanently changing the scene's narration.

**Attach To:**  
Any GameObject in the scene where temporary narration is required.

**Public Variables / Inspector Settings:**  
- `tempMessages` (string[]) – Array of temporary text messages to display.  
- `tempAudioClips` (AudioClip[]) – Corresponding audio clips for each message.  

**Dependencies:**  
- Requires a `NarrationManager` in the scene to manage messages and audio.  
- Uses `NarrationManager.OnNarrationComplete` to detect when playback finishes.

**Functions / Example Usage:**  
1. `PlayTemporaryNarration()` – Replaces the current narration with the temporary messages and starts playback.  
2. `OnTemporaryNarrationComplete()` – Restores the original messages and audio once temporary narration is done.

**Notes:**  
- Automatically finds the first active `NarrationManager` in the scene.  
- If no `NarrationManager` is present, an error is logged.  
- Designed to be non-destructive, so original narration resumes after temporary messages play.
