# Shared Player Controller

This is the ready-to-use player for the arcade hub and minigames. `FppController` handles movement, sprinting, gravity, grounding, and Platformer jumping. Cinemachine handles where the camera sits, how it follows, and Third Person wall avoidance.

The useful files are:

- Prefab: `Assets/Shared/FPPController-NoCam.prefab`
- Movement: `Assets/Shared/Scripts/FppController.cs`
- Camera coordinator: `Assets/Shared/Scripts/FppController.Camera.cs`
- Input actions: `Assets/Shared/InputSystem_Actions.inputactions`

Despite its old name, the prefab includes the real gameplay camera and all five Cinemachine rigs.

## Quick Setup

Drag `FPPController-NoCam.prefab` into the scene, place it slightly above a floor collider, and choose `Camera Mode` on its `FppController`.

The selected camera mode automatically selects its movement rules. There is no second movement-mode setting to keep in sync.

Keep only one active shared player, gameplay camera, and Audio Listener in a scene. The output camera stays tagged `MainCamera` because the grab and outline systems use `Camera.main`.

## Controls

- Move: `WASD`, arrow keys, or gamepad left stick
- Look or orbit: mouse or gamepad right stick
- Jump: `Space` or gamepad south button, Platformer only
- Sprint: `Left Shift` or gamepad left-stick press
- Unlock or relock the cursor: `Escape`

Grab-object rotation temporarily suppresses camera look, so mouse movement can rotate the held object instead.

## Camera And Movement Modes

### First Person

Uses `CinemachineFollow` and `CinemachinePanTilt`. The camera sits at the upper-body target and freely looks around.

Movement uses the full camera-relative ground plane. The player turns toward movement, sprint works with meaningful forward input, and jumping is disabled.

### Third Person

Uses `CinemachineOrbitalFollow`, `CinemachineRotationComposer`, and `CinemachineDeoccluder`.

The camera freely orbits around the upper-body target. Movement is camera-relative, the player turns toward movement, sprint works forward, and jumping is disabled. The Deoccluder pulls the camera around or in front of walls that block the player.

### Top Down

Uses a fixed orthographic `CinemachineFollow` camera above the player.

Movement works in all camera-relative ground directions, sprint works in any meaningful direction, and jumping is disabled. This mode does not use look input, so the cursor stays unlocked.

### Isometric

Uses an orthographic `CinemachineOrbitalFollow` and `CinemachineRotationComposer`.

The camera can orbit horizontally while its elevated angle stays fixed. Movement is camera-relative, sprint works forward, and jumping is disabled. It intentionally has no collision extension so the orthographic framing stays stable.

### Platformer

Uses a fixed side-on orthographic `CinemachineFollow`.

Only left and right movement are accepted. Sprint works in either horizontal direction, and this is the only mode that allows jumping.

## Tuning Cinemachine

Expand `CinemachineRig` inside the shared prefab to tune a mode. Each child `CinemachineCamera` owns its own lens and follow behavior.

- Change a camera's Lens settings for field of view or orthographic size.
- Change `Follow Offset` on TopDown or Platformer to move their fixed framing.
- Change `Radius` and axis ranges on an `Orbital Follow` to tune orbit distance and limits.
- Keep Isometric's vertical orbit axis disabled if its pitch should remain fixed.
- Tune ThirdPerson's `Cinemachine Deoccluder` to change wall avoidance.
- Keep the Player layer excluded from collision and grounding masks.

The real `FppCam` only renders the final result. Do not position it manually; `CinemachineBrain` moves it to the active Cinemachine camera.

Mode changes use a hard cut. This avoids strange blends between perspective and orthographic cameras.

## Movement Feel

Movement accelerates toward the requested speed rather than snapping instantly. `Acceleration` controls starting and reversing, `Deceleration` controls stopping, and `Air Acceleration` controls steering while airborne.

Platformer jumping includes:

- `Coyote Time`, which allows a jump just after leaving a ledge.
- `Jump Buffer Time`, which remembers a jump pressed just before landing.
- Variable jump height, where releasing jump early produces a shorter jump.
- Stronger falling gravity, which keeps the jump from feeling floaty.

Other modes ignore jump input but keep gravity, so falling from ledges and switching modes while airborne still behave normally.

## Pseudocode Overview

The main movement flow stays independent from Cinemachine:

```text
apply a changed camera mode
read and clamp movement input
apply the movement and sprint rules linked to that mode
process Platformer jump timers when allowed
apply gravity
move the CharacterController once
```

Camera-relative movement uses the real output camera:

```text
camera right = output camera right flattened onto the ground
camera forward = output camera forward flattened onto the ground
desired movement = right * horizontal input + forward * vertical input

turn player toward desired movement
```

Cinemachine input is filtered before it reaches orbit or Pan Tilt:

```text
if cursor is unlocked or grab rotation is active:
    return no look input

if active look device is a pointer:
    apply mouse delta sensitivity without frame-rate scaling
else:
    apply controller look speed per second
```

Changing modes only switches which Cinemachine camera is active:

```text
disable the previous mode camera
enable the selected mode camera
reset the Cinemachine Brain for an immediate cut
lock or unlock the cursor for the selected mode
```

## Grab And Outline Compatibility

The output `FppCam` remains tagged `MainCamera`, so the grab and outline managers continue to raycast from the final Cinemachine-controlled view.

Crosshair interaction usually feels best in First Person or Third Person. Mouse-ray interaction may suit Top Down, Isometric, or Platformer minigames better.

## Troubleshooting

- Nothing renders: make sure `FppCam` is enabled and has `CinemachineBrain`.
- Camera does not follow: check that the selected mode camera is assigned on `FppController`.
- Multiple cameras fight: keep only one active shared player and output camera.
- Mouse does not look: click the Game view or press `Escape` to relock the cursor.
- Third Person clips through walls: check its Deoccluder and collision layers.
- Third Person detects the player as a wall: exclude the Player layer and keep `Ignore Tag` set to `Player`.
- Isometric pitch changes: keep its vertical input axis disabled and its vertical orbit range fixed.
- Player cannot jump outside Platformer: this is expected.
- Player cannot jump in Platformer: check the floor collider and `Ground Layers`.
- Player constantly appears grounded: exclude the Player layer from `Ground Layers`.
- Grab or outline raycasts fail: keep `FppCam` tagged `MainCamera`.

For most minigames, choose a mode and tune only that mode's Cinemachine child. The shared movement defaults can then stay familiar across the arcade.
