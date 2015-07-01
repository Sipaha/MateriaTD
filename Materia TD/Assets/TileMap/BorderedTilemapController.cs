using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(TileMap), typeof(Grid))]
public class BorderedTilemapController : MonoBehaviour
{
    public const int Top = 1, Right = 2, Bottom = 4, Left = 8;

    [SerializeField]
    private Brush[] brushes = new Brush[16];
    [SerializeField]
    private int[] samplesBorders = { 0, 
                                Left, 
                                Left | Right, 
                                Left | Top, 
                                Top | Right | Bottom, 
                                Top | Right | Bottom | Left };

    [SerializeField]
    private bool _isReady = false;
    public bool IsReady
    {
        get { return _isReady; }
        private set { _isReady = value; }
    }

    private TileMap _map;
    private TileMap Map
    {
        get
        {
            if (_map == null) _map = GetComponent<TileMap>();
            return _map;
        }
    }

    private Grid _grid;
    private Grid Grid
    {
        get
        {
            if (_grid == null) _grid = GetComponent<Grid>();
            return _grid;
        }
    }

    private bool _locked = false;
    public bool Locked
    {
        get { return _locked; }
        set
        {
            if (value != _locked)
            {
                _locked = value;
                Grid.DrawGrid = !value;
            }
        }
    }

    public Texture2D GetSampleTexture(int sampleIdx)
    {
        Brush brush = brushes[samplesBorders[sampleIdx]];
        return brush != null ? brush.Sprite.texture : null;
    }

    public void AddBrush(Sprite sprite, int sampleIdx)
    {
        int borders = samplesBorders[sampleIdx];
        int shiftedBorders = borders;
        int angle = 0;
        do
        {
			if(brushes[shiftedBorders] != null) 
			{
				DestroyImmediate(brushes[shiftedBorders]);
			}
            brushes[shiftedBorders] = new Brush(sprite) { Angle = angle };
            angle -= 90;
            shiftedBorders = ShiftClockwise(shiftedBorders);
        } while (shiftedBorders != borders);

        IsReady = !brushes.Any(b => b == null);
        //if(IsReady) RecalculateMap();
    }

    public bool HaveBorder(int sampleId, int side)
    {
        return (samplesBorders[sampleId] & side) > 0;
    }

    public void AddTile(Vector2 loc)
    {
        if (!_isReady) return;
        IntVec2 pos = Grid.ToCoordinates(loc);
        GameObject tile = Map.GetTile(pos);
        if (tile == null)
        {
            int borders = GetBorders(pos);
            tile = brushes[borders].Create();
            Map.AddTile(pos, tile);
            tile.transform.position = Grid.ToWorld(pos);
            UpdateNeighbours(pos);
            //Debug.Log("New tile added to " + pos);
        }
    }

    public void RemoveTile(Vector2 loc)
    {
        IntVec2 pos = Grid.ToCoordinates(loc);
        GameObject removedTile = Map.RemoveTile(pos);
        if (removedTile != null)
        {
            DestroyImmediate(removedTile);
            UpdateNeighbours(pos);
        }
    }

    public bool ContainsTile(Vector2 loc)
    {
        return Map.Contains(Grid.ToCoordinates(loc));
    }

    public void UpdateNeighbours(IntVec2 pos)
    {
        UpdateTile(pos.Left);
        UpdateTile(pos.Right);
        UpdateTile(pos.Up);
        UpdateTile(pos.Down);
    }

    public void UpdateTile(IntVec2 pos)
    {
        GameObject tile = Map.GetTile(pos);
        if (tile != null)
        {
            int borders = GetBorders(pos);
            brushes[borders].Apply(tile);
        }
    }

    public void RecalculateMap()
    {
        foreach (KeyValuePair<IntVec2, GameObject> pair in Map)
        {
            int borders = GetBorders(pair.Key);
            brushes[borders].Apply(pair.Value);
        }
    }

    private int GetBorders(IntVec2 pos)
    {
        int borders = 0;
        borders |= Map.GetTile(pos.Left) == null ? Left : 0;
        borders |= Map.GetTile(pos.Right) == null ? Right : 0;
        borders |= Map.GetTile(pos.Down) == null ? Top : 0;
        borders |= Map.GetTile(pos.Up) == null ? Bottom : 0;
        return borders;
    }

    private int ShiftClockwise(int borders)
    {
		int leftBit = (borders & Left) > 0 ? 1 : 0;
		int baseBits = borders & 7;
		int result = baseBits << 1 | leftBit;
		return result;
	}

	void OnDestroy() 
	{
		for (int i = 0; i < brushes.Length; i++) 
		{
			DestroyImmediate(brushes[i]);
		}
	}
}