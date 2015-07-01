using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

    public enum GridType { SQUARE, HEXAGON, ISOMETRIC }
    public Vector2 CellSize = new Vector2(1, 1);
    public Vector2 PosScale = new Vector2(1, -1);
    public IntVec2 Size = new IntVec2(10, 7);
    public GridType Type = GridType.SQUARE;
    public Color Color = new Color(26.0f / 255, 148.0f / 255, 26.0f / 255, 72.0f / 255);
    public bool DrawGrid = true;

    public IntVec2 ToCoordinates(Vector2 vec)
    {
        switch (Type)
        {
            case GridType.SQUARE: return ToSquareCoords(vec);
            case GridType.HEXAGON: return ToHexagonCoords(vec);
            case GridType.ISOMETRIC: return ToIsometricCoords(vec);
            default: return new IntVec2();
        }
    }

    public Vector2 ToWorld(IntVec2 vec)
    {
        switch (Type)
        {
            case GridType.SQUARE: return SquareToWorld(vec);
            case GridType.HEXAGON: return HexagonToWorld(vec);
            case GridType.ISOMETRIC: return IsometricToWorld(vec);
            default: return new Vector2();
        }
    }

    private Vector2 SquareToWorld(IntVec2 vec)
    {
        Vector3 mapPos = transform.position;
        Vector3 scale = transform.localScale;
        float x = (vec.X + 0.5f) * CellSize.x * scale.x / PosScale.x + mapPos.x;
        float y = (vec.Y + 0.5f) * CellSize.y * scale.y / PosScale.y + mapPos.y;
        return new Vector2(x, y);
    }

    private Vector2 HexagonToWorld(IntVec2 vec)
    {
        return new Vector2();
    }

    private Vector2 IsometricToWorld(IntVec2 vec)
    {
        return new Vector2();
    }

    private IntVec2 ToSquareCoords(Vector2 vec)
    {
        Vector3 mapPos = transform.position;
        Vector3 scale = transform.localScale;
        int tileX = Mathf.FloorToInt(((vec.x - mapPos.x) * PosScale.x) / (CellSize.x * scale.x));
        int tileY = Mathf.FloorToInt(((vec.y - mapPos.y) * PosScale.y) / (CellSize.y * scale.y));
        return new IntVec2(tileX, tileY);
    }

    private IntVec2 ToHexagonCoords(Vector2 vec)
    {
        return new IntVec2();
    }

    private IntVec2 ToIsometricCoords(Vector2 vec)
    {
        return new IntVec2();
    }

    public Vector2 Snap(Vector2 vec)
    {
        return ToWorld(ToCoordinates(vec));
    }

    void OnDrawGizmos()
    {
        if (DrawGrid)
        {
            Vector3 pos = transform.position;
            Vector3 scale = transform.localScale;
            Gizmos.color = Color;

            float x0 = pos.x;
            float x1 = pos.x + Size.X * scale.x * PosScale.x;
            float scaleY = PosScale.y * scale.y;
            for (float y = 0; y <= Size.Y; y += CellSize.y)
            {
                float scaledY = pos.y + y * scaleY;
                Gizmos.DrawLine(new Vector3(x0, scaledY, 0.0f),
                                new Vector3(x1, scaledY, 0.0f));
            }
            float y0 = pos.y;
            float y1 = pos.y + Size.Y * scale.y * PosScale.y;
            float scaleX = PosScale.x * scale.x;
            for (float x = 0; x <= Size.X; x += CellSize.x)
            {
                float scaledX = pos.x + x * scaleX;
                Gizmos.DrawLine(new Vector3(scaledX, y0, 0.0f),
                                new Vector3(scaledX, y1, 0.0f));
            }
        }
    }
}
