using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct IntVec2
{
    public IntVec2 Left { get { return new IntVec2(X - 1, Y); } }
    public IntVec2 Right { get { return new IntVec2(X + 1, Y); } }
    public IntVec2 Up { get { return new IntVec2(X, Y + 1); } }
    public IntVec2 Down { get { return new IntVec2(X, Y - 1); } }

    public int X, Y;

    public IntVec2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public IntVec2(float x, float y)
    {
        X = (int)x;
        Y = (int)y;
    }

    public int SqrDistance(IntVec2 point)
    {
        int deltaX = point.X - X;
        int deltaY = point.Y - Y;
        return deltaX * deltaX + deltaY * deltaY;
    }

    public int SubDistance(IntVec2 point)
    {
        int deltaX = point.X - X;
        int deltaY = point.Y - Y;
        return Math.Abs(deltaX) + Math.Abs(deltaY);
    }

    public float DistanceTo(IntVec2 point)
    {
        int deltaX = point.X - X;
        int deltaY = point.Y - Y;
        return Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public static IntVec2 operator +(IntVec2 v1, IntVec2 v2)
    {
        return new IntVec2(v1.X + v2.X, v1.Y + v2.Y);
    }

    public static IntVec2 operator -(IntVec2 v1, IntVec2 v2)
    {
        return new IntVec2(v1.X - v2.X, v1.Y - v2.Y);
    }

    public static implicit operator Vector2(IntVec2 point)
    {
        return new Vector2(point.X, point.Y);
    }

	public override string ToString ()
	{
		return string.Format ("Point2D: [{0}, {1}]", X, Y);
	}
}
