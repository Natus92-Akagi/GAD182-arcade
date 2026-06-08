# Liminal Arcade

Group Unity project containing a shared arcade hub and each team member's microgame work.

## Project

- Unity: `6000.0.76f1`
- Rendering: Universal Render Pipeline
- Input: Unity Input System
- Main scene: `Assets/Shared/Scenes/Sc_ArcadeExterior.unity`
- Input actions: `Assets/Shared/InputSystem_Actions.inputactions`

## Asset Ownership

- `Assets/Shared`: systems and assets used by the full project.
- `Assets/Diego`, `Assets/Finn`, `Assets/Jd`, `Assets/Josh`: member-owned work.

Keep work in your own folder until it needs to be shared. Check with the team before changing another member's files or shared systems.

## Shared Player

The shared player uses `Player.Controller`, Cinemachine, and `Assets/Shared/Controller.prefab`. It supports First Person, Third Person, Top Down, Isometric, and Platformer camera modes with movement rules and tuning linked to each mode.

Setup, camera modes, tuning, and troubleshooting:

- `Assets/Shared/Scripts/README Controller.md`

- Move: `WASD` or arrow keys
- Look: mouse
- Jump: `Space` in First Person, Third Person, Isometric, and Platformer
- Sprint: `Left Shift`
- Interact: `E`
- Grab: hold left mouse button
- Adjust held distance: mouse wheel
- Freeze aimed grabbable: right mouse button
- Toggle held-object rotation: middle mouse button

Keep the gameplay camera tagged `MainCamera`; the grab and outline systems use `Camera.main` as a fallback.

## JD System Guides

- `Assets/Jd/Scripts/MazeGen/README MazeGen.md`
- `Assets/Jd/Scripts/PhysGrab/README PhysGrab.md`
- `Assets/Jd/Scripts/Outline/README Outline.md`

Before making a build, add every required scene to the Build Profiles scene list and test the final executable.
