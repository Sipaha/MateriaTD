using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System;

[CustomEditor(typeof(BorderedTilemapController))]
public class BorderedTileMapEditor : Editor
{
    const int leftRightMargin = 30, minTilesMargin = 10, maxTilesMargin = 30, 
              maxTileSize = 60, tilesExternalMargin = 20;

    private BorderedTilemapController _controller;

    private Tool _lastTool = Tool.None;

    private int hotControl;

    private bool _isInitialized = false;
    private bool _isTileMapSelected = false;

    private Rect[] samplesRects = new Rect[6];

    public void OnEnable()
    {
        _controller = (BorderedTilemapController)target;

        if (!_controller.Locked)
        {
            TurnOffTool();
        }

        if (!_isInitialized)
        {
            SceneView.onSceneGUIDelegate += Input;
            _isInitialized = true;
        }

        _isTileMapSelected = true;
    }

    public void OnDisable()
    {
        TurnOnTool();
        _isTileMapSelected = false;
    }

    private void TurnOffTool()
    {
        _lastTool = Tools.current;
        Tools.current = Tool.None;
    }

    private void TurnOnTool()
    {
        if (Tools.current == Tool.None)
        {
            Tools.current = _lastTool;
        }
    }

    private void Input(SceneView sceneview)
    {
        if (!_isTileMapSelected || _controller.Locked || EditorApplication.isPlaying) return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;
        EventType type = e.type;

        if ((type == EventType.MouseDown || type == EventType.MouseDrag) && (e.button == 0 || e.button == 1))
        {
            Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 mousePos = r.origin;

            switch (e.button)
            {
                case 0:
                    _controller.AddTile(mousePos);
                    break;
                case 1:
                    _controller.RemoveTile(mousePos);
                    break;
            }
            e.Use();
        }
    }

    private static UnityEngine.Object GetDroppedGraphicFile()
    {
        EventType eventType = Event.current.type;

        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            UnityEngine.Object droppedObject = DragAndDrop.objectReferences[0];
            Type type = droppedObject.GetType();
            bool correctType = type == typeof(Texture2D) || type == typeof(Sprite);
            DragAndDrop.visualMode = correctType ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

            if (eventType == EventType.DragPerform && correctType)
            {
                DragAndDrop.AcceptDrag();
                Event.current.Use();
                return droppedObject;
            }
        }
        return null;
    }

    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Locked");
        _controller.Locked = EditorGUILayout.Toggle(_controller.Locked, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Ready");
        EditorGUILayout.Toggle(_controller.IsReady, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        float horizontalSize = Screen.width - 2f * leftRightMargin;
        float tileSize = Math.Min((horizontalSize-2f*minTilesMargin) / 3f, maxTileSize);
        float tilesMargin = Math.Min((horizontalSize - 3f * tileSize) / 2f, maxTilesMargin);

        Rect rect = GUILayoutUtility.GetLastRect();
        float top = rect.y + rect.height + tilesExternalMargin;
        float left = (Screen.width - 3 * tileSize - 2 * tilesMargin) / 2f;
        rect.y = top;
        rect.x = left;
        rect.size = new Vector2(tileSize, tileSize);

        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                int idx = row * 3 + col;
                samplesRects[idx] = rect;
                Texture2D texture = _controller.GetSampleTexture(idx);
                EditorGUI.DrawPreviewTexture(rect, texture != null ? texture : Texture2D.whiteTexture);
                DrawBorders(rect, idx);
                rect.x += tileSize + tilesMargin;
            }
            rect.x = left;
            rect.y += tileSize + tilesMargin;
        }

        GUILayout.Space(rect.y - tilesMargin - top + 2*tilesExternalMargin);

        UnityEngine.Object droppedFile = GetDroppedGraphicFile();
        if (droppedFile != null)
        {
            Sprite tileSprite = null;
            if (droppedFile is Texture2D)
            {
                string assetPath = AssetDatabase.GetAssetPath(droppedFile);
                tileSprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
            }
            else if (droppedFile is Sprite)
            {
                tileSprite = droppedFile as Sprite;
            }

            if (tileSprite != null)
            {
                for (int i = 0; i < samplesRects.Length; i++)
                {
                    if (samplesRects[i].Contains(Event.current.mousePosition))
                    {
                        _controller.AddBrush(tileSprite, i);
                        break;
                    }
                }
            }
        }

        SceneView.RepaintAll();
    }

    private void DrawBorders(Rect rect, int sampleIdx)
    {
        Handles.BeginGUI();
        Handles.color = Color.black;
        if (_controller.HaveBorder(sampleIdx, BorderedTilemapController.Left))
        {
            Handles.DrawLine(new Vector3(rect.xMin - 3, rect.yMin - 3), 
                             new Vector3(rect.xMin - 3, rect.yMax + 3));
        }
        if (_controller.HaveBorder(sampleIdx, BorderedTilemapController.Right))
        {
            Handles.DrawLine(new Vector3(rect.xMax + 3, rect.yMin - 3), 
                             new Vector3(rect.xMax + 3, rect.yMax + 3));
        }
        if (_controller.HaveBorder(sampleIdx, BorderedTilemapController.Top))
        {
            Handles.DrawLine(new Vector3(rect.xMin - 3, rect.yMin - 3), 
                             new Vector3(rect.xMax + 3, rect.yMin - 3));
        }
        if (_controller.HaveBorder(sampleIdx, BorderedTilemapController.Bottom))
        {
            Handles.DrawLine(new Vector3(rect.xMin - 3, rect.yMax + 3), 
                             new Vector3(rect.xMax + 3, rect.yMax + 3));
        }
        Handles.EndGUI();
    }

    /*

    private TileMap _tileMap;

    private Tool _lastTool = Tool.None;
    private string[] _sortingLayers;

    private bool Locked
    {
        get { return _tileMap.Locked; }
        set
        {
            if (_tileMap.Locked != value)
            {
                if (value)
                {
                    TurnOnTool();
                    SceneView.onSceneGUIDelegate -= Input;
                }
                else
                {
                    TurnOffTool();
                    SceneView.onSceneGUIDelegate += Input;
                }
            }
            _tileMap.Locked = value;
        }
    }

    private bool _initialized;
    private bool tileMapSelected;

    private int hotControl;

    public void OnEnable()
    {
        _tileMap = (TileMapOld)target;

        Type internalEditorUtilityType = typeof(InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        _sortingLayers = (string[])sortingLayersProperty.GetValue(null, new object[0]);

        if (!Locked)
        {
            TurnOffTool();
        }

        if (!_initialized)
        {
            SceneView.onSceneGUIDelegate += Input;
            _initialized = true;
        }

        tileMapSelected = true;
    }

    public void OnDisable()
    {
        TurnOnTool();

        tileMapSelected = false;
    }

    private void TurnOffTool()
    {
        _lastTool = Tools.current;
        Tools.current = Tool.None;
    }

    private void TurnOnTool()
    {
        if (Tools.current == Tool.None)
        {
            Tools.current = _lastTool;
        }
    }

    private void Input(SceneView sceneview)
    {
        if (!tileMapSelected || Locked || EditorApplication.isPlaying) return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;
        EventType type = e.type;

        if ((type == EventType.MouseDown || type == EventType.MouseDrag) && (e.button == 0 || e.button == 1))
        {
            Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 mousePos = r.origin;

            switch (e.button)
            {
                case 0:
                    _tileMap.AddTile(mousePos);
                    break;
                case 1:
                    _tileMap.Clear(mousePos);
                    break;
            }
            e.Use();
        }
        else
        {
            UnityEngine.Object droppedFile = GetDroppedGraphicFile();
            if (droppedFile != null)
            {
                Sprite tileSprite = null;
                if (droppedFile is Texture2D)
                {
                    string assetPath = AssetDatabase.GetAssetPath(droppedFile);
                    tileSprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;
                }
                else if (droppedFile is Sprite)
                {
                    tileSprite = droppedFile as Sprite;
                }

                if (tileSprite != null)
                {

                }
            }
        }
    }

    private static UnityEngine.Object GetDroppedGraphicFile()
    {
        EventType eventType = Event.current.type;

        if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
        {
            UnityEngine.Object droppedObject = DragAndDrop.objectReferences[0];
            Type type = droppedObject.GetType();
            bool correctType = type == typeof(Texture2D) || type == typeof(Sprite);
            DragAndDrop.visualMode = correctType ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

            if (eventType == EventType.DragPerform && correctType)
            {
                DragAndDrop.AcceptDrag();
                Event.current.Use();
                return droppedObject;
            }
        }
        return null;
    }

    public override void OnInspectorGUI()
    {
        _tileMap.MapSize = EditorGUILayout.Vector2Field("Map size", _tileMap.MapSize);
        _tileMap.CellSize = EditorGUILayout.Vector2Field("Cell size", _tileMap.CellSize);
        _tileMap.PosScale = EditorGUILayout.Vector2Field("Pos scale", _tileMap.PosScale);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Locked");
        Locked = EditorGUILayout.Toggle(Locked, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Enable grid");
        _tileMap.DrawGrid = EditorGUILayout.Toggle(_tileMap.DrawGrid, GUILayout.Width(50));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Grid color");
        _tileMap.GridColor = EditorGUILayout.ColorField(_tileMap.GridColor, GUILayout.Width(Screen.width * 0.641f));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        _tileMap.SortingLayer = EditorGUILayout.Popup("Sorting layer", _tileMap.SortingLayer, _sortingLayers);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Order in layer");
        _tileMap.SortingOrder = EditorGUILayout.IntField(_tileMap.SortingOrder);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Remove selected tile"))
        {
            _tileMap.RemoveSelectedBrush();
        }

        UnityEngine.Object[] droppedTiles = GetDroppedFiles(_tileFilters);
        if (droppedTiles != null)
        {
            foreach (UnityEngine.Object obj in droppedTiles)
            {
                string assetPath = AssetDatabase.GetAssetPath(obj);
                Sprite tileSprite = null;
                if (obj is Texture2D)
                {
                    tileSprite = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Sprite)) as Sprite;

                }
                else if (obj is Sprite)
                {
                    tileSprite = obj as Sprite;
                }
                if (tileSprite != null)
                {
                    _tileMap.AddBrush(tileSprite);
                }
            }
        }

        Texture2D[] palette = _tileMap.PaletteTextures;
        if (palette != null)
        {
            float height = 60 * ((palette.Length / 3) + (palette.Length % 3 > 0 ? 1 : 0));
            _tileMap.Selection = GUILayout.SelectionGrid(_tileMap.Selection, palette, 3,
                                                        GUILayout.Width(Screen.width - 20),
                                                        GUILayout.Height(height));
        }

        SceneView.RepaintAll();
    }
*/
}