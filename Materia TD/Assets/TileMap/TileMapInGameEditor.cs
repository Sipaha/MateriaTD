using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BorderedTilemapController))]
public class TileMapInGameEditor : MonoBehaviour {

    private BorderedTilemapController _controller;
    public Transform Cursor;

    void Start()
    {
        if (Cursor == null)
        {
            Debug.Log("Cursor not setup in tilemapingameeditor!!!");
        }
        _controller = GetComponent<BorderedTilemapController>();
    }

    public void ChangeTile()
    {
        Vector2 loc = Cursor.position;
        if (_controller.ContainsTile(loc))
        {
            _controller.RemoveTile(loc);
            //player platforms ++
        }
        else
        {
            _controller.AddTile(loc);
            //player platforms --
        }
        
    }
}
