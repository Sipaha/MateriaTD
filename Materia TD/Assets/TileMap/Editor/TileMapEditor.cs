using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;
using UnityEditorInternal;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : Editor {

    private TileMap _tileMap;
    private string[] _sortingLayers;

    public void OnEnable()
    {
        _tileMap = (TileMap)target;

        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        _sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        _tileMap.SortingLayer = EditorGUILayout.Popup("Sorting layer", _tileMap.SortingLayer, _sortingLayers);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Order in layer");
        _tileMap.SortingOrder = EditorGUILayout.IntField(_tileMap.SortingOrder);
        GUILayout.EndHorizontal();
    }
}
