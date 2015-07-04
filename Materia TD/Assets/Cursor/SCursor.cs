using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SCursor : MonoBehaviour {

    public delegate void PositionChangedHandler(SCursor cursor);
    public event PositionChangedHandler OnPositionChange;

    public GameObject SelectedTile;
    public GameObject SelectedTower;

    private Grid _grid;
    private TileMap _map;
    public IntVec2 Coordinates { get; private set; }

	void Start () {
        _grid = transform.parent.GetComponent<Grid>();
        _map = transform.parent.GetComponent<TileMap>();
        Coordinates = new IntVec2(-1, -1);
        transform.position = _grid.ToWorld(Coordinates);
    }

    public void SetPosition(Vector2 loc)
    {
        IntVec2 pos = _grid.ToCoordinates(loc);
        if (!pos.Equals(Coordinates))
        {
            SelectedTile = _map.GetTile(pos);
            SelectedTower = null;//todo
            transform.position = _grid.ToWorld(pos);
            Coordinates = pos;
            if (OnPositionChange != null)
            {
                OnPositionChange.Invoke(this);
            }
        }

        
    }
}
