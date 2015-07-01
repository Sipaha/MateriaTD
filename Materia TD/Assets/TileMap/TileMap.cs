using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMap : MonoBehaviour, IEnumerable, ISerializationCallbackReceiver
{
    [SerializeField]
    private int _sortingLayer;
    public int SortingLayer
    {
        get { return _sortingLayer; }
        set
        {
            if (value != _sortingLayer)
            {
                foreach (Transform child in transform)
                {
                    child.GetComponent<SpriteRenderer>().sortingLayerID = value;
                }
                _sortingLayer = value;
            }
        }
    }
    [SerializeField]
    private int _sortingOrder = 1;
    public int SortingOrder
    {
        get { return _sortingOrder; }
        set
        {
            if (value != _sortingOrder)
            {
                foreach (Transform child in transform)
                {
                    child.GetComponent<SpriteRenderer>().sortingOrder = value;
                }
                _sortingOrder = value;
            }
        }
    }

    private readonly Dictionary<IntVec2, GameObject> _tiles = new Dictionary<IntVec2, GameObject>();
    [SerializeField]
    private List<IntVec2> serializedTilesVectors = new List<IntVec2>();
    [SerializeField]
    private List<GameObject> serializedTiles = new List<GameObject>();

    public GameObject GetTile(IntVec2 point)
    {
        GameObject tile;
        return _tiles.TryGetValue(point, out tile) ? tile : null;
    }

    public void AddTile(IntVec2 pos, GameObject tile)
    {
        _tiles.Add(pos, tile);
        tile.transform.parent = transform;
    }

    public GameObject RemoveTile(IntVec2 pos)
    {
        GameObject tile = GetTile(pos);
        if (tile != null)
        {
            _tiles.Remove(pos);
            return tile;
        }
        else return null;
    }

    public void RemoveTile(GameObject tile)
    {
        IntVec2 key = new IntVec2();
        foreach (KeyValuePair<IntVec2, GameObject> pair in _tiles)
        {
            if (pair.Value == tile)
            {
                key = pair.Key;
                break;
            }
        }
        _tiles.Remove(key);
    }

    public IEnumerator GetEnumerator()
    {
        foreach (KeyValuePair<IntVec2, GameObject> pair in _tiles)
        {
            yield return pair;
        }
    }

    public void OnAfterDeserialize()
    {
        _tiles.Clear();
        for (int i = 0; i < serializedTiles.Count; i++)
        {
            _tiles.Add(serializedTilesVectors[i], serializedTiles[i]);
        }
        serializedTilesVectors.Clear();
        serializedTiles.Clear();
    }

    public void OnBeforeSerialize()
    {
        serializedTilesVectors.Clear();
        serializedTiles.Clear();
        foreach(KeyValuePair<IntVec2, GameObject> pair in _tiles) {
            serializedTilesVectors.Add(pair.Key);
            serializedTiles.Add(pair.Value);
        }
    }

    public bool Contains(IntVec2 pos)
    {
        return GetTile(pos) != null;
    }
}
