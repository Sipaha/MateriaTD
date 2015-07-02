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

    public IntVec2[] FindPath(IntVec2 from, IntVec2 to, bool[,] pathMap, IntVec2[] moveDirections)
    {
        openNodes.Clear();
        closedNodes.Clear();

        Node searchNode = new Node();
        searchNode.point = from;
        bool endReached = false;

        Plane2D directPath = new Plane2D(from, to);
       
        openNodes.Add(searchNode, 0);

        while (openNodes.Size > 0)
        {
            searchNode = openNodes.Pop();

            if (searchNode.point.Equals(to)) {
                endReached = true;
                break;
            }

            IntVec2 p = searchNode.point;

            foreach (IntVec2 direction in moveDirections)
            {
                IntVec2 newPos = p + direction;
                if (newPos.X >= 0 && newPos.X < pathMap.GetLength(0) &&
                    newPos.Y >= 0 && newPos.Y < pathMap.GetLength(1) &&
                    pathMap[newPos.X, newPos.Y] && !closedNodes.Contains(newPos))
                {
                    AddOpenNode(searchNode, newPos, to, directPath);
                }
            }
            closedNodes.Add(searchNode.point);
        }

        if (endReached)
        {
            IntVec2[] path = new IntVec2[searchNode.steps + 1];
            for (int i = searchNode.steps; i >= 0; i--)
            {
                path[i] = searchNode.point;
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