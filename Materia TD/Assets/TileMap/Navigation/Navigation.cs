using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Navigation : MonoBehaviour {

    public TileMap tileMap;
    public Grid grid;
    public Transform target;

    private IntVec2 currentPos;
    private IntVec2 targetPos;
    private PathFinder pathFinder = new PathFinder();
    private bool[,] pathMap;
    public Vector2[] path;

	void Start () {
        IntVec2 gridSize = grid.Size;
        pathMap = new bool[gridSize.X, gridSize.Y];
        for (int x = 0; x < gridSize.X; x++)
        {
            for (int y = 0; y < gridSize.Y; y++)
            {
                pathMap[x, y] = true;
            }
        }
        foreach(KeyValuePair<IntVec2, GameObject> tile in tileMap) {
            pathMap[tile.Key.X, tile.Key.Y] = false;
        }
        currentPos = grid.ToCoordinates(transform.position);
        targetPos = grid.ToCoordinates(target.position);
        RecalculatePath();
        tileMap.OnChange += OnMapChanged;
	}

    void OnMapChanged(IntVec2 pos, GameObject tile)
    {
        pathMap[pos.X, pos.Y] = tile == null;
        RecalculatePath();
    }

    void RecalculatePath()
    {
        if(!pathMap[currentPos.X, currentPos.Y] || !pathMap[targetPos.X, targetPos.Y]) return;
        IntVec2[] coordinatesPath = pathFinder.FindPath(currentPos, targetPos, pathMap, grid.GetMoveDirections());
        path = new Vector2[coordinatesPath.Length];
        for (int i = 0; i < path.Length; i++)
        {
            path[i] = grid.ToWorld(coordinatesPath[i]);
        }
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = 1; i < path.Length; i++)
            {
                Gizmos.DrawLine(path[i-1], path[i]);
            }
        }
    }
}
