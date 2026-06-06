using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sol //this script will control and manage the visibility of walls and the creation of a continuous path through the maze.
{ 

public class Room3D : MonoBehaviour
{
    public enum Directions
    {
        NORTH,
        SOUTH,
        EAST,
        WEST,
        NONE,
    }

    [SerializeField] GameObject NorthWall;
    [SerializeField] GameObject SouthWall;
    [SerializeField] GameObject EastWall;
    [SerializeField] GameObject WestWall;
    [SerializeField] GameObject roofObject;

    Dictionary<Directions, GameObject> walls =
      new Dictionary<Directions, GameObject>();

    bool wallsInitialized = false;

    public Vector3Int Index { get; set; }

    public bool visited { get; set; } = false;

    Dictionary<Directions, bool> dirFlags =
      new Dictionary<Directions, bool>();
    
    private void Awake()
    {
        InitializeWalls();
    }

    private void InitializeWalls()
    {
        if (wallsInitialized)
        {
            return;
        }

        walls.Clear();
        walls[Directions.NORTH] = NorthWall;
        walls[Directions.SOUTH] = SouthWall;
        walls[Directions.EAST] = EastWall;
        walls[Directions.WEST] = WestWall;

        if (roofObject != null)
        {
            roofObject.SetActive(true);
        }

        wallsInitialized = true;
    }

    private void SetActive(Directions dir, bool flag)
    {
        InitializeWalls();

        if (!walls.TryGetValue(dir, out GameObject wall) || wall == null)
        {
            Debug.LogWarning($"{name} is missing a wall reference for {dir}.");
            return;
        }

        wall.SetActive(flag);
    }

    public void SetDirFlag(Directions dir, bool flag)
    {
        if (dir == Directions.NONE)
        {
            return;
        }

        dirFlags[dir] = flag;
        SetActive(dir, flag);
    }
}
}
