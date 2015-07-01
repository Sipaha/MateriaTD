using UnityEngine;

[RequireComponent (typeof(MeshRenderer), typeof(MeshFilter))]
public class ColoredQuad : MonoBehaviour {

    public Vector2 Pivot = new Vector2(0.5f,0.5f);

    public Color LeftColor;
    public Color RightColor;
}
