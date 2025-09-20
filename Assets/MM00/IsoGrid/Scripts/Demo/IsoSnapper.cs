#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Mixmotion00.Grid;
using System;

[ExecuteInEditMode]
public class IsoCrossSnapper : MonoBehaviour
{
    public float cellW = 1f;
    public float cellH = 0.5f;
    public int rows = 10;
    public int cols = 10;

    void Update()
    {
        if (!Application.isPlaying)
        {
            transform.position = GetSnappedPosition(transform.position);
        }
    }

    //[Header("DebugGizmos")]
    //public Vector2 center;
    //public int size = 2;

    //private void OnDrawGizmos()
    //{
    //    var a = new GridObject(Vector2.zero, new Vector2(1, 1));
    //    //var dmdRange = GridLocator.GetIsoRange(a.Pos, 2, 2, 1);
    //    //var dmdRange = GridLocator.GetIsoRange(center, size, 2, 1);
    //    var dmdRange = GridLocator.GetIsoRange2(center, size, 2, 1);

    //    Action<DrawLine> drawLine = (dl) =>
    //    {
    //        Debug.DrawLine(dl.From, dl.To, dl.Color);
    //    };

    //    foreach (var item in dmdRange)
    //    {
    //        var cross = new Cross(item, 0.5f, Color.yellow);
    //        GridRenderer.DrawCross(cross, drawLine);
    //    }
    //}

    Vector3 GetSnappedPosition(Vector3 worldPos)
    {
        return Mixmotion00.Grid.GridRenderer.GetSnappedPosition(worldPos, rows, cols, cellW, cellH);

        //// --- Step 1: shift into grid space centered at (0,0) ---
        //float gridWidth = cols * cellW;
        //float gridHeight = rows * cellH;

        //Vector3 local = worldPos;
        //local.x += gridWidth / 2f - cellW / 2f;
        //local.y += gridHeight / 2f - cellH / 2f;

        //// --- Step 2: compute indices for base grid ---
        //int j = Mathf.RoundToInt(local.x / cellW);
        //int i = Mathf.RoundToInt(local.y / cellH);

        //Vector3 oriCross = new Vector3(j * cellW, i * cellH, worldPos.z);
        //oriCross.x -= gridWidth / 2f - cellW / 2f;
        //oriCross.y -= gridHeight / 2f - cellH / 2f;

        //// --- Step 3: compute indices for offset grid ---
        //int j2 = Mathf.RoundToInt((local.x - cellW / 2f) / cellW);
        //int i2 = Mathf.RoundToInt((local.y - cellH / 2f) / cellH);

        //Vector3 offCross = new Vector3(j2 * cellW + cellW / 2f, i2 * cellH + cellH / 2f, worldPos.z);
        //offCross.x -= gridWidth / 2f - cellW / 2f;
        //offCross.y -= gridHeight / 2f - cellH / 2f;

        //// --- Step 4: pick nearest ---
        //float dOri = Vector2.SqrMagnitude(worldPos - oriCross);
        //float dOff = Vector2.SqrMagnitude(worldPos - offCross);

        //return (dOri <= dOff) ? oriCross : offCross;
    }
}
#endif
