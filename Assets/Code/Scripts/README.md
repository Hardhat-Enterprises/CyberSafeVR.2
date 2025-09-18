# Global Scripts
**Namespace:** Global

## BackgroundMusic.cs

**Purpose:**  
Manages background music playback in the game. Ensures a single persistent instance of music that continues across scene loads and prevents duplicate music objects.

**Attach To:**  
A GameObject containing an `AudioSource` component with the desired background music clip.

**Behavior / Example Usage:**  
1. On `Awake()`, checks if an instance of `BackgroundMusic` already exists.  
   - If no instance exists, sets this object as the persistent music instance using `DontDestroyOnLoad`.  
   - If an instance already exists, destroys the duplicate object to prevent overlapping music.  
2. Starts playing the audio immediately if an `AudioSource` is present and not already playing.  

**Notes:**  
- Only one `BackgroundMusic` object should exist in the entire game.  
- Designed for background tracks that should continue seamlessly across multiple scenes.  
- The `AudioSource` component should have the desired clip assigned in the Inspector.

## CanvasFollower.cs

**Purpose:**  
Makes a canvas or UI element follow a target (usually the VR camera) at a fixed distance and slight vertical offset while always facing the user.

**Attach To:**  
Any GameObject containing a canvas or UI element that should follow the player.

**Public Variables / Inspector Settings:**  
- `target` (Transform) – The object to follow, typically the XR camera.  
- `distance` (float) – Distance in front of the camera.  
- `heightOffset` (float) – Vertical offset from the target position.  
- `followSpeed` (float) – Speed of the smooth following motion.

**Behavior / Example Usage:**  
1. On `Update()`, calculates the target position based on the target’s forward direction, distance, and height offset.  
2. Smoothly interpolates the canvas position using `Lerp` toward the target position.  
3. Rotates the canvas to face the target, ensuring UI is always readable.

**Notes:**  
- Designed for HUDs, prompts, or interactive UI elements that should stay in front of the user.  
- Smooth movement prevents sudden jumps or jittering of the canvas.  
- Ensure the target (camera or player) is correctly assigned in the Inspector.

### Clipping.cs

**Purpose:**  
Keeps the VR player’s `CharacterController` aligned with the head position to prevent clipping through walls or other obstacles in the scene.

**Attach To:**  
A GameObject containing both a `CharacterController` and `XROrigin`.

**Requirements:**  
- `CharacterController` component  
- `XROrigin` component (from Unity XR Toolkit)

**Behavior / Example Usage:**  
1. On `Awake()`, caches references to the `CharacterController` and the local camera from `XROrigin`.  
   - If either component is missing, the script disables itself and logs a warning.  
2. On `Update()`:  
   - Updates the `CharacterController.center` to match the head’s local X and Z positions, keeping the Y-center unchanged.  
   - Calls `SimpleMove(Vector3.zero)` to ensure the CharacterController’s physics are updated every frame.  

**Notes:**  
- Ensures smooth movement and collision prevention as the player moves their head in VR.  
- Does not move the player directly; it only keeps the CharacterController centered on the headset.  
- Essential for preventing VR clipping issues in physically constrained environments.

## PauseMenu.cs

**Purpose:**  
Handles pausing and resuming the game in VR, providing a menu UI and input handling for both VR controllers and editor testing.

**Attach To:**  
A GameObject in your scene (typically a manager object) with a reference to the pause menu UI.

**Public Fields:**  
- `pauseMenuUI` — The GameObject containing the pause menu UI. Must be assigned in the inspector.

**Behavior / Example Usage:**  
1. **Pause Input Detection:**  
   - Monitors the `menuButton` on both left and right VR controllers.  
   - Supports editor testing via the `M` key.  

2. **Pause / Resume Logic:**  
   - `ShowPauseMenu()` — Activates the UI and sets `Time.timeScale = 0` to freeze gameplay.  
   - `ResumeGame()` — Hides the UI and resumes gameplay (`Time.timeScale = 1`).  
   - `TogglePauseMenu()` — Switches between paused and resumed states.  

3. **Game Management:**  
   - `RestartGame()` — Reloads the current scene and resumes time.  
   - `QuitGame()` — Loads the "MainMenu" scene and resumes time.  

**Device Handling:**  
- Automatically reinitializes input devices if a new VR controller connects.

**Notes:**  
- `Time.timeScale` is used to pause the game; physics and animations are effectively frozen while paused.  
- Ensure `pauseMenuUI` is assigned in the inspector, otherwise the script cannot show/hide the menu.  
- Can be extended with additional buttons or events for VR menus.

## SceneLoader.cs

**Purpose:**  
Provides simple methods to load scenes or quit the application. Typically used with UI buttons.

**Attach To:**  
A GameObject in your scene (commonly a UI manager or button handler object).

**Public Methods:**  
- `LoadScene(string sceneName)`  
  Loads the scene with the given name. Use this as the OnClick event for a button to switch scenes.  

- `Quit()`  
  Exits the application. In the editor, it stops play mode; in a built build, it closes the application.  

**Usage Notes:**  
- Ensure the target scenes are added to the **Build Settings** in Unity.  
- Can be connected to UI buttons via the Inspector using the OnClick event.

## SceneSelectionManager.cs

**Purpose:**  
Manages the selection and loading of scenes in the project. Implements a singleton pattern so the selected scene persists across the session.

**Attach To:**  
A GameObject that also has the `SceneLoader` component.

**Key Features:**  
- **Singleton**: Only one instance exists to maintain the selected scene.
- **Scene Selection**: Stores the scene name selected by the user.
- **Scene Loading**: Calls `SceneLoader` to load the selected scene.

**Public Methods:**  
- `SetSelectedScene(string sceneName)`  
  Stores the name of the scene to be loaded.  

- `StartSelectedScene()`  
  Loads the selected scene via the `SceneLoader`.  
  If no scene is selected, defaults to `"SafeWebBrowsing01"`.  

**Usage Notes:**  
- Ensure a `SceneLoader` component exists on the same GameObject.  
- Use UI buttons or other inputs to call `SetSelectedScene` before calling `StartSelectedScene`.  
- Scene names must match exactly with the names in **Build Settings**.
