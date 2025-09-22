using UnityEngine;
using Mixmotion00.Grid;
using Mixmotion00.Grid.Pathfinding;
using System;
using System.Collections.Generic;

public class DemoPathfinding : MonoBehaviour
{
    public Vector2 From, Goal;
    [Header("Cell Configh")]
    public DemoDrawGridMesh demoGridMesh;

    private void OnDrawGizmos()
    {


        //var nodeFrom = new Node();
        //nodeFrom.Pos = From;

        //var nodeGoal = new Node();
        //nodeGoal.Pos = Goal;

        //var result = IsoPathfinding.ProcessNode(nodeFrom, nodeGoal);
        //H = result.HCost;

        //Action<DrawLine> drawLine = (dl) =>
        //{
        //    Debug.DrawLine(dl.From, dl.To, dl.Color);
        //};

        //float size = 0.5f;
        //GridRenderer.DrawCross(new Cross(From, size, Color.green), drawLine);
        //GridRenderer.DrawCross(new Cross(Goal, size, Color.yellow), drawLine);


        DisplayPath();
    }

    public string DebugStr = "";

    private void DisplayPath()
    {
        if (From == Goal || demoGridMesh == null) return;

        var allPoints = new List<Vector2>();
        demoGridMesh.VisibleCrossess.ForEach(v => allPoints.Add(v.Pos));

        var shortest = IsoPathfinding.ShortestPath(allPoints, From, Goal, 1, demoGridMesh.CellSzW, demoGridMesh.CellSzH);

        Action<DrawLine> drawLine = (dl) =>
        {
            Debug.DrawLine(dl.From, dl.To, dl.Color);
        };

        foreach (var node in shortest)
        {
            GridRenderer.DrawCross(new Cross(node.Pos, 0.5f, Color.green), drawLine);
        }

        DebugStr = $"count: {demoGridMesh.VisibleCrossess.Count}";

        //IsoPathfinding.ShortestPath()
    }
}
