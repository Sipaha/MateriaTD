using UnityEngine;
using System.Collections.Generic;

public class PathFinder {
    
    public class Node
    {
        public IntVec2 point;
        public int steps;
        public Node from;

		public override bool Equals (object obj)
		{
			return obj is Node && ((Node)obj).point.Equals(point);
		}
		public override int GetHashCode ()
		{
			return point.GetHashCode ();
		}
    }

    private BinaryHeap<Node> openNodes = new BinaryHeap<Node>();
    private HashSet<IntVec2> closedNodes = new HashSet<IntVec2>();

    public Vector3[] FindPath(Vector3 from, Vector3 to, TileMap map, Grid grid)
    {
        IntVec2 fromPoint = grid.ToCoordinates(from);
        IntVec2 toPoint = grid.ToCoordinates(to);

        openNodes.Clear();
        closedNodes.Clear();

        Node searchNode = new Node();
        searchNode.point = fromPoint;
        bool endReached = false;

        Plane2D directPath = new Plane2D(fromPoint, toPoint);
       
        openNodes.Add(searchNode, 0);

        while (openNodes.Size > 0)
        {
            searchNode = openNodes.Pop();

            if (searchNode.point.Equals(toPoint)) {
                endReached = true;
                break;
            }

            IntVec2 p = searchNode.point;

            IntVec2 left = p.Left;
            IntVec2 right = p.Right;
            IntVec2 up = p.Up;
            IntVec2 down = p.Down;

            if (map.Contains(left) && !closedNodes.Contains(left)) 
			{
                AddOpenNode(searchNode, left, toPoint, directPath);
			}
            if (map.Contains(right) && !closedNodes.Contains(right)) 
			{
                AddOpenNode(searchNode, right, toPoint, directPath);
			}
            if (map.Contains(down) && !closedNodes.Contains(down)) 
			{
                AddOpenNode(searchNode, down, toPoint, directPath);
			}
            if (map.Contains(up) && !closedNodes.Contains(up)) 
			{
                AddOpenNode(searchNode, up, toPoint, directPath);
			}

            closedNodes.Add(searchNode.point);
        }

        if (endReached)
        {
            Vector3[] path = new Vector3[searchNode.steps + 1];
            for (int i = searchNode.steps; i >= 0; i--)
            {
                path[i] = grid.ToWorld(searchNode.point);
                searchNode = searchNode.from;
            }

            return path;
        }

        return null;
    }

    private void AddOpenNode(Node from, IntVec2 newPoint, IntVec2 finish, Plane2D directPath)
    {
        Node node = new Node();
        node.point = newPoint;
        node.steps = from.steps + 1;
        node.from = from;

		float distanceToDirectPath = 0.001f * Mathf.Abs (directPath.distanceTo (newPoint));
		float nodeValue = 100 * (newPoint.SubDistance(finish) + node.steps) + distanceToDirectPath;

        Node replacedNode;

        if (openNodes.Add(node, nodeValue, out replacedNode))
        {
            if (replacedNode != null)
            {
                closedNodes.Add(replacedNode.point);
            }
        }
        else
        {
            closedNodes.Add(node.point);
        }
    }
}