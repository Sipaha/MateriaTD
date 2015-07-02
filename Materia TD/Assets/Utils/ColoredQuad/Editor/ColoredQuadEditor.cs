using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;

[CustomEditor (typeof(ColoredQuad))]
public class ColoredQuadEditor : Editor {

    private string[] _sortingLayers;
    private MeshRenderer _renderer;
    private MeshFilter _meshFilter;
    private ColoredQuad _quad;

    public void OnEnable()
    {
        _quad = (ColoredQuad)target;
        _renderer = _quad.GetComponent<MeshRenderer>();
        _meshFilter = _quad.GetComponent<MeshFilter>();

        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        _sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
		EditorGUI.BeginChangeCheck ();

		int order = EditorGUILayout.Popup("Sorting layer", _renderer.sortingLayerID, _sortingLayers);
        if (EditorGUI.EndChangeCheck ()) 
		{
			_renderer.sortingLayerID = order;
			//_renderer.sortingLayerName = _sortingLayers[order];
		}
		GUILayout.EndHorizontal();

		_quad.Pivot = EditorGUILayout.Vector2Field("Pivot", _quad.Pivot);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Order in layer");
        _renderer.sortingOrder = EditorGUILayout.IntField(_renderer.sortingOrder);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Left color");
        _quad.LeftColor = EditorGUILayout.ColorField(_quad.LeftColor, GUILayout.Width(Screen.width * 0.641f));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Right color");
        _quad.RightColor = EditorGUILayout.ColorField(_quad.RightColor, GUILayout.Width(Screen.width * 0.641f));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Generate"))
        {
			DestroyImmediate(_meshFilter.sharedMesh);
            _meshFilter.sharedMesh = CreateMesh();
        }
    }

    private Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

		float left = -_quad.Pivot.x;
		float right = left + 1f;
		float bottom = -_quad.Pivot.y;
		float top = bottom + 1f;

        Vector3[] vertices =
		{	
			new Vector3( left, bottom, 0),
			new Vector3( right, top, 0),
			new Vector3( right, bottom, 0),
			new Vector3(left, top, 0)
        };

        int[] triangles = 
        {
             0, 1, 2,
             1, 0, 3,
        };
        
        Color[] colors = 
        {
            _quad.LeftColor,
            _quad.RightColor,
            _quad.RightColor,
            _quad.LeftColor
        };

        Vector3[] normals = 
        {
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1),
            new Vector3(0, 0, -1)
        };

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.colors = colors;

        return mesh;
    }
}
