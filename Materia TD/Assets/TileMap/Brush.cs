using System;
using System.Collections.Generic;
using UnityEngine;

public class Brush : ScriptableObject
{
    public Sprite Sprite;

    public float Angle;
    public Vector2 Scale = new Vector2(1f, 1f);
    
    public Brush(Sprite sprite)
    {
        Sprite = sprite;
    }

    public GameObject Create()
    {
        GameObject tile = new GameObject();
        tile.hideFlags = HideFlags.HideInHierarchy;
        tile.AddComponent<SpriteRenderer>().sprite = Sprite;
        tile.transform.localScale = new Vector3(Scale.x, Scale.y, 1f);
        tile.transform.eulerAngles = new Vector3(0, 0, Angle);
        return tile;
    }

    public void Apply(GameObject tile)
    {
        tile.GetComponent<SpriteRenderer>().sprite = Sprite;
        tile.transform.localScale = new Vector3(Scale.x, Scale.y, 1f);
        tile.transform.eulerAngles = new Vector3(0, 0, Angle);
    }

    public Brush Set(Brush brush)
    {
        Sprite = brush.Sprite;
        Angle = brush.Angle;
        Scale = brush.Scale;
        return this;
    }
}