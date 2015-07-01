using UnityEngine;
using System.Collections;

public struct Plane2D {

    public Vector2 normal;
    public float offset;

    public Plane2D(Vector2 from, Vector2 to)
    {
        Vector2 direction = (to - from).normalized;
		normal = new Vector2 (direction.y, -direction.x);
		offset = normal.x * from.x + normal.y * from.y;
    }

    public Plane2D(IntVec2 from, IntVec2 to)
    {
        Vector2 fromVec = new Vector2(from.X, from.Y);
        Vector2 toVec = new Vector2(to.X, to.Y);
        Vector2 direction = (toVec - fromVec).normalized;
        normal = new Vector2(direction.y, -direction.x);
        offset = normal.x * fromVec.x + normal.y * fromVec.y;
    }

    public float distanceTo(Vector2 point)
    {
		return normal.x * point.x + normal.y * point.y - offset;
    }
}
