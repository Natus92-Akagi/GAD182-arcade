# Jd PhysGrab Components

Simple first-person physics grabbing for the arcade scene.

## Namespaces

Use this when writing code that talks to the grab system:

```csharp
using Sol.Grab;
```

Main types:

```csharp
Sol.Grab.GrabManager
Sol.Grab.GrabbableComponent
Sol.Grab.GrabMode
Sol.Grab.GrabInputBinding
Sol.Grab.HoldDistanceOrigin
```

## GrabManager

Put one `GrabManager` in the scene. In our setup it is already on the `FPPController` prefab.

What it does:

- Casts a ray from the crosshair or mouse.
- Finds objects with `GrabbableComponent`.
- Holds the object while the grab input is held.
- Releases the object when the grab input is released.
- Lets the scroll wheel move the held object closer/farther.

Important inspector fields:

- `Raycast Distance`: how far the player can grab from.
- `Raycast Layer Mask`: which layers can be hit. Keep the `Player` layer off so the ray does not hit the player body.
- `Is Grabbing Enabled`: turn grabbing on/off.
- `Scroll Sensitivity`: how fast scroll changes hold distance.
- `Grab Input`: `Attack` means left click; `Interact` means the interact input.
- `Grab Mode`: `Crosshair` for first-person use, `Mouse` for cursor-based use.
- `Gameplay Camera`: assign the player's camera. If empty, it uses `Camera.main`.
- `Rotation Mode`: if on, held objects rotate instead of moving.
- `Is Locking Enabled`: allows right click freeze/unfreeze.
- `Allow Middle Click Rotation Toggle`: lets middle click turn rotation mode on/off.
- `Is Frozen`: freezes/unfreezes all grabbable rigidbodies.

## GrabbableComponent

Put `GrabbableComponent` on an object that the player should be able to pick up.

Requirements:

- The object must have a `Collider`.
- The object should have a `Rigidbody`, either on the same object or on a parent object.
- Objects without `GrabbableComponent` are not grabbable.

Important inspector fields:

- `Hold Distance`: default distance when used by other scripts.
- `Follow Speed`: how quickly the object moves toward the hold point.

## Quick Setup For A Prop

1. Select the prop GameObject.
2. Add a `Collider` if it does not already have one.
3. Add a `Rigidbody`.
4. Add `GrabbableComponent`.
5. Press Play.
6. Aim the crosshair at the object.
7. Hold left click to grab it.
8. Release left click to drop it.
9. Use scroll wheel while holding it to change distance.

## Useful Code Examples

Check what the player is holding:

```csharp
using Sol.Grab;
using UnityEngine;

public class GrabDebug : MonoBehaviour
{
    private void Update()
    {
        if (GrabManager.Instance != null && GrabManager.Instance.HeldObject != null)
            Debug.Log("Holding: " + GrabManager.Instance.HeldObject.name);
    }
}
```

Force the player to drop the held object:

```csharp
using Sol.Grab;
using UnityEngine;

public class DropButton : MonoBehaviour
{
    public void Drop()
    {
        if (GrabManager.Instance != null)
            GrabManager.Instance.ForceRelease();
    }
}
```

## Common Problems

- If the object does not grab, check that it has `GrabbableComponent`.
- If the object does not move, check that it has a non-kinematic `Rigidbody`.
- If the ray hits the player, make sure the `Player` layer is turned off in `Raycast Layer Mask`.
- If left click does nothing, check that `Grab Input` is set to `Attack`.
