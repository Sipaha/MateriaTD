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
}