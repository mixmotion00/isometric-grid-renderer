using Mixmotion00.Grid;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DemoGridEdge : MonoBehaviour
{
    [Header("Cell Configh")]
    public float cellW = 1f;
    public float cellH = 0.5f;
    [Header("DebugGizmos A")]
    public Vector2 a_center;
    public int a_size = 2;

    private void OnDrawGizmos()
    {
        var a = new GridObject(a_center, a_size);

        //DrawGridObject(a, Color.yellow);

        Action<DrawLine> drawLine = (dl) =>
        {
            Debug.DrawLine(dl.From, dl.To, dl.Color);
        };

        var edges = GridLocator.GetIsoEdge(a_center, a_size, cellW, cellH);
        foreach (var edge in edges) 
        {
            var cross = new Cross(edge, 0.5f, Color.red);
            GridRenderer.DrawCross(cross, drawLine);
        }

        //if (GridLocator.IsOverlap(a, b, cellW, cellH, out List<Vector2> overlaps))
        //{
        //    Action<DrawLine> drawLine = (dl) =>
        //    {
        //        Debug.DrawLine(dl.From, dl.To, dl.Color);
        //    };

        //    foreach (var item in overlaps)
        //    {
        //        var cross = new Cross(item, 0.5f, Color.red);
        //        GridRenderer.DrawCross(cross, drawLine);
        //    }
        //}
    }

    private void DrawGridObject(GridObject grObj, Color color)
    {
        var dmdRange = GridLocator.GetIsoRange(grObj.Pos, grObj.Size, 2, 1);

        Action<DrawLine> drawLine = (dl) =>
        {
            Debug.DrawLine(dl.From, dl.To, dl.Color);
        };

        foreach (var item in dmdRange)
        {
            var cross = new Cross(item, 0.5f, color);
            GridRenderer.DrawCross(cross, drawLine);
        }
    }
}
