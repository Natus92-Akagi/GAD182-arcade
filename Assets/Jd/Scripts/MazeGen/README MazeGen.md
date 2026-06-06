# JD MazeGen

2D and 3D grid maze generators using recursive backtracking.

## Files

- 2D demo: `Assets/Jd/Scenes/Sc_MazeGen2D.unity`
- 3D demo: `Assets/Jd/Scenes/Sc_MazeGen3D.unity`
- 2D rooms: `Assets/Jd/Prefabs/2DRooms`
- 3D rooms: `Assets/Jd/Prefabs/3DRooms`
- Pregenerated mazes: `Assets/Jd/Prefabs/Pregenerated`

## Namespace And Types

```csharp
using Sol;
```

- `Dun_Gen2D`: creates a 2D maze.
- `Room2D`: stores a 2D room's wall references.
- `DunGen3D`: creates a 3D maze.
- `Room3D`: stores a 3D room's wall and optional roof references.

Note that `Dun_Gen2D` includes an underscore.

## 2D Setup

1. Add `Dun_Gen2D` to an empty GameObject.
2. Assign a prefab with `Room2D` on its root.
3. Set `Num X` and `Num Y`.
4. Tag the scene camera `MainCamera`.

The room prefab requires:

- Child `SpriteRenderer` components used to calculate room spacing.
- Assigned north, south, east, and west wall GameObjects.

The included prefab is `Assets/Jd/Prefabs/2DRooms/Room2D.prefab`.

`Dun_Gen2D` currently uses legacy `UnityEngine.Input` for its `R` shortcut, which does not match this project's Input System-only setup. Call `CreateDungeon()` from another script when needed.

## 3D Setup

1. Add `DunGen3D` to an empty GameObject.
2. Add room variants to `Possible Room Prefabs`.
3. Set `Num X` and `Num Z`.
4. Enter Play mode and press `R` to regenerate.

Each room prefab requires:

- `Room3D` on its root.
- Assigned north, south, east, and west wall GameObjects.
- Enabled child `Renderer` components used to calculate room spacing.
- The same width and length as every other room variant.

`Roof Object` is optional. Invalid prefabs are skipped. Generated rooms are placed under a `Generated Rooms` child.

## Code API

```csharp
[SerializeField] private DunGen3D maze;

public void Regenerate()
{
    maze.CreateDungeon();
}
```

Use `Dun_Gen2D` for the 2D generator.

## Troubleshooting

- No rooms: assign a valid room prefab.
- Missing openings: assign all four wall references.
- Overlaps or gaps: make room dimensions consistent.
- Incorrect spacing: check the prefab's renderer bounds.
- 2D camera error: tag the camera `MainCamera`.
