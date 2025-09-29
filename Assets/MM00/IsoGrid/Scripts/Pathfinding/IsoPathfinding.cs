using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mixmotion00;
using System;

namespace Mixmotion00.Grid.Pathfinding
{
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }

    public class Node : IHeapItem<Node>
    {
        public Node Parent { get; set; }
        public Vector2 Pos { get; set; }
        public float HCost { get; set; } //approx to the goal
        public float GCost { get; set; } //node from start

        public float FCost { get { return HCost + GCost; } }

        public int HeapIndex { get; set; }

        public Node() { }

        public Node(Node parent, int h, int g, Vector2 pos)
        {
            Parent = parent;
            HCost = h;
            GCost = g;
            Pos = pos;
        }

        public void SetCost(int gCost, int hCost)
        {
            GCost = gCost;
            HCost = hCost;
        }

        // Compare nodes by FCost (and HCost as tiebreaker)
        public int CompareTo(Node other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if (compare == 0) compare = HCost.CompareTo(other.HCost); // tie-breaker
            return -compare; // because heap usually works with max priority
        }
    }

    public static class IsoPathfinding
    {
        public static Node ProcessNode(Node current, Node goal)
        {
            float dx = Mathf.Abs(current.Pos.x - goal.Pos.x);
            float dy = Mathf.Abs(current.Pos.y - goal.Pos.y);

            // Heuristic (Diagonal)
            int H = (int)Mathf.Max(dx, dy);

            // G cost
            //int stepCost = (neighborIsDiagonal ? 14 : 10); // use 10/14 instead of 1/√2 to avoid floats
            //int G = parent.G + stepCost;

            current.HCost = H;

            return current;
        }

        public static List<Node> ShortestPath(List<Vector2> points, Vector2 start, Vector2 goal,
            int size, float cellW, float cellH)
        {
            // Create nodes
            var allNodes = new List<Node>(); //inclusive start and goal
            Node startNode = new();
            Node goalNode = new();

            foreach (var pt in points)
            {
                Node node = new Node(null, 0, 0, pt);
                allNodes.Add(node);

                if (pt == start)
                    startNode = node;
                if (pt == goal)
                    goalNode = node;
            }

            // Nodes to explore
            var open = new Heap<Node>(allNodes.Count);
            open.Add(startNode);
            // Nodes already checked
            var closed = new HashSet<Node>();

            while (open.Count > 0)
            {
                // pick lowest F
                //Node current = GetLowestF(open);
                Node current = open.RemoveFirst();

                // check if reach the goal
                if (current.Pos == goalNode.Pos)
                    return ReconstructPath(goalNode); // reconstruct path

                // move from open to close
                //open.Remove(current);
                closed.Add(current);

                var neighbors = GetNeighbors(current, allNodes, size, cellW, cellH);

                // check neighbors
                foreach (var node in neighbors)
                {
                    if (closed.Contains(node))
                        continue;

                    float tentativeG = current.GCost + Distance(current, node);

                    // If new path to neighbor is better OR not yet in openList
                    if (tentativeG < node.GCost || !open.Contains(node))
                    {
                        node.Parent = current;
                        node.GCost = tentativeG;
                        node.HCost = Heuristic(node, goalNode);

                        if (!open.Contains(node))
                            open.Add(node);
                        else
                            open.UpdateItem(node); // ✅ Make sure heap reorders correctly

                        //if (!open.Contains(node))
                        //    open.Add(node);
                    }
                }
            }

            return null; //no path found
        }

//        # old unused since changing to heap
//        private static Node GetLowestF(List<Node> nodes)
//        {
//            Node min = nodes[0];
//            foreach (Node n in nodes)
//            {
//                if (n.FCost < min.FCost)
//                    min = n;
//            }

//            return min;
//        }

        // #old 29/9/2025 - without using heap
        //public static List<Node> ShortestPath(List<Vector2> points, Vector2 start, Vector2 goal,
        //    int size, float cellW, float cellH)
        //{
        //    // Create nodes
        //    var allNodes = new List<Node>(); //inclusive start and goal
        //    Node startNode = new();
        //    Node goalNode = new();

        //    foreach (var pt in points)
        //    {
        //        Node node = new Node(null, 0, 0, pt);
        //        allNodes.Add(node);

        //        if (pt == start)
        //            startNode = node;
        //        if (pt == goal)
        //            goalNode = node;
        //    }

        //    // Nodes to explore
        //    var open = new List<Node> { startNode };
        //    // Nodes already checked
        //    var closed = new List<Node>();

        //    while (open.Count > 0)
        //    {
        //        // pick lowest F
        //        Node current = GetLowestF(open);

        //        // check if reach the goal
        //        if (current.Pos == goalNode.Pos)
        //            return ReconstructPath(goalNode); // reconstruct path

        //        // move from open to close
        //        open.Remove(current);
        //        closed.Add(current);

        //        var neighbors = GetNeighbors(current, allNodes, size, cellW, cellH);

        //        // check neighbors
        //        foreach (var node in neighbors)
        //        {
        //            if (closed.Contains(node))
        //                continue;

        //            float tentativeG = current.GCost + Distance(current, node);

        //            // If new path to neighbor is better OR not yet in openList
        //            if (!open.Contains(node) || tentativeG < node.GCost)
        //            {
        //                node.Parent = current;
        //                node.GCost = tentativeG;
        //                node.HCost = Heuristic(node, goalNode);

        //                if (!open.Contains(node))
        //                    open.Add(node);
        //            }
        //        }
        //    }

        //    return null; //no path found
        //}

        // #old unused since changing to heap
        //private static Node GetLowestF(List<Node> nodes)
        //{
        //    Node min = nodes[0];
        //    foreach (Node n in nodes)
        //    {
        //        if (n.FCost < min.FCost)
        //            min = n;
        //    }

        //    return min;
        //}

        private static float Distance(Node a, Node b)
        {
            // 1 for straight, 1.414 for diagonal
            float dx = Mathf.Abs(a.Pos.x - b.Pos.x);
            float dy = Mathf.Abs(a.Pos.y - b.Pos.y);
            return (dx + dy == 2) ? 1.414f : 1f;
        }

        private static float Heuristic(Node a, Node b)
        {
            // Choose heuristic: Manhattan, Diagonal, Euclidean
            return Mathf.Abs(a.Pos.x - b.Pos.x) + Mathf.Abs(a.Pos.y - b.Pos.y);
        }

        private static List<Node> GetNeighbors(Node node, List<Node> allNodes, int size, float cellW, float cellH)
        {
            var isoNeighbors = GridLocator.GetIsoRange(node.Pos, size, cellW, cellH);
            isoNeighbors.Remove(node.Pos);

            var neighborNodes = new List<Node>();

            foreach (var item in allNodes)
            {
                if (isoNeighbors.Any(pos => pos == item.Pos))
                    neighborNodes.Add(item);
            }

            neighborNodes.Remove(node); //for safety

            return neighborNodes;
        }

        private static List<Node> ReconstructPath(Node goal)
        {
            List<Node> path = new List<Node>();
            Node current = goal;
            while (current != null)
            {
                path.Add(current);
                current = current.Parent;
            }
            path.Reverse();
            return path;
        }
    }
}