# Jd Outline Components

Simple crosshair hover outlines for objects in the arcade scene.

## Namespaces

Use this when writing code that talks to the outline system:

```csharp
using Sol.Outline;
```

If you also need the shared `GrabMode` enum, add:

```csharp
using Sol.Grab;
```

Main types:

```csharp
Sol.Outline.OutlineManager
Sol.Outline.OutlineComponent
Sol.Outline.SolOutlineRendererFeature
Sol.Grab.GrabMode
```

## OutlineManager

Put one `OutlineManager` in the scene. In our setup it is already on the `FPPController` prefab.

What it does:

- Casts a ray from the crosshair or mouse.
- Finds objects with `OutlineComponent`.
- Shows the outline while the player is looking at the object.
- Hides the outline when the player looks away.

Important inspector fields:

- `Raycast Distance`: how far the player can detect outline objects.
- `Detection Layer Mask`: which layers can be hit. Keep the `Player` layer off so the ray does not hit the player body.
- `Ray Mode`: `Crosshair` for first-person use, `Mouse` for cursor-based use.
- `Gameplay Camera`: assign the player's camera. If empty, it uses `Camera.main`.

## OutlineComponent

Put `OutlineComponent` on an object that should be able to show an outline.

Requirements:

- The object or one of its children must have a `Renderer`.
- Objects without `OutlineComponent` do not outline.
- For a grabbable prop, you can use both `GrabbableComponent` and `OutlineComponent` on the same object.

Important inspector fields:

- `Outline Color`: the color of the outline.
- `Outline Width`: thickness of the outline in pixels.
- `Always Visible`: outline is always shown, not only on hover.
- `Priority`: outline draws on top of everything, even if blocked by another object.

## SolOutlineRendererFeature

This is the URP renderer feature that actually draws the outline on screen.

Current setup:

- It is already added to `Assets/Shared/Settings/PC_Renderer.asset`.
- It uses the hidden shaders:
  - `Hidden/Arcade/OutlineMask`
  - `Hidden/Arcade/OutlineFullscreen`

If outlines stop rendering:

1. Open `Assets/Shared/Settings/PC_Renderer.asset`.
2. Check that `SolOutlineRendererFeature` is listed in Renderer Features.
3. Check that the scene is using the PC render pipeline asset.

## Quick Setup For A Prop

1. Select the prop GameObject.
2. Make sure it has a visible mesh renderer, or a child with a renderer.
3. Add `OutlineComponent`.
4. Pick an `Outline Color`.
5. Press Play.
6. Aim the crosshair at the object.
7. The outline should appear while looking at it.

## Useful Code Examples

Turn an outline on manually:

```csharp
using Sol.Outline;
using UnityEngine;

public class ManualOutlineExample : MonoBehaviour
{
    [SerializeField] private OutlineComponent outline;

    public void Show()
    {
        outline.ShowOutline();
    }

    public void Hide()
    {
        outline.HideOutline();
    }
}
```

Find the object currently outlined by the player:

```csharp
using Sol.Outline;
using UnityEngine;

public class OutlineDebug : MonoBehaviour
{
    private void Update()
    {
        if (OutlineManager.Instance != null && OutlineManager.Instance.CurrentOutlinedObject != null)
            Debug.Log("Looking at: " + OutlineManager.Instance.CurrentOutlinedObject.name);
    }
}
```

## Common Problems

- If nothing outlines, check that `SolOutlineRendererFeature` is on the active URP renderer.
- If one object does not outline, check that it has `OutlineComponent`.
- If the outline ray hits the player, make sure the `Player` layer is turned off in `Detection Layer Mask`.
- If the outline is hidden behind walls, enable `Priority` on that object's `OutlineComponent`.
