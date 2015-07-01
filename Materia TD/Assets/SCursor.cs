using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SCursor : MonoBehaviour {

    public GameObject SelectedTile;

    private Grid _grid;
    private TileMap _map;

	void Start () {
        _grid = transform.parent.GetComponent<Grid>();
        _map = transform.parent.GetComponent<TileMap>();
    }

    public void SetPosition(Vector2 loc)
    {
        IntVec2 pos = _grid.ToCoordinates(loc);
        SelectedTile = _map.GetTile(pos);
        transform.position = _grid.ToWorld(pos);
    }
}
