using Mixmotion00.Grid;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DemoDrawGridMesh : MonoBehaviour
{
    public float width = 0.05f;
    public Material lineMaterial;

    private Mesh mesh;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Color> colors = new List<Color>();

    public float CellSzW = 1f;
    public float CellSzH = 1f;

    private void OnDrawGizmos()
    {
        // #uncomment if need debug
        //Action<DrawLine> drawLine = (dl) =>
        //{
        //    Debug.DrawLine(dl.From, dl.To, dl.Color);
        //};

        //GridRenderer.DrawGrid(10, 10, 1, 1, drawLine, Color.white, Color.red);
    }

    private void Start()
    {
        InitIsoGrid();
        lastCamPos = Camera.main.transform.position;
    }

    Vector3 lastCamPos;

    private void Update()
    {
        if (Vector3.Distance(Camera.main.transform.position, lastCamPos) > 0.001f)
        {
            // Camera moved significantly
            CreateLineMesh();

            Action<DrawLine> drawLine = (dl) =>
            {
                AddLine(dl.From, dl.To, dl.Color);
            };

            var drawLimit = new DrawLimit(Camera.main.transform.position, dlW, dlH);

            GridRenderer.DrawGrid(100000, 100000, CellSzW, CellSzH, drawLine, Color.white, Color.red, drawLimit);

            ApplyMesh();


            lastCamPos = Camera.main.transform.position;
        }
    }

    public Vector2 dlCenter;
    public float dlW;
    public float dlH;

    [ContextMenu("InitIsoGrid")]
    private void InitIsoGrid()
    {
        CreateLineMesh();

        Action<DrawLine> drawLine = (dl) =>
        {
            AddLine(dl.From, dl.To, dl.Color);
        };

        var drawLimit = new DrawLimit(dlCenter, dlW, dlH);

        GridRenderer.DrawGrid(100000, 100000, CellSzW, CellSzH, drawLine, Color.white, Color.red, drawLimit);

        ApplyMesh();
    }

    private void CreateLineMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        // Default material if none provided
        if (lineMaterial == null)
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
        GetComponent<MeshRenderer>().material = lineMaterial;
    }

    public void AddLine(Vector2 from, Vector2 to, Color col)
    {
        Vector3 dir = (to - from).normalized;
        Vector3 normal = Vector3.Cross(dir, Vector3.forward) * (width * 0.5f);

        int vIndex = vertices.Count;

        // 4 verts per line
        vertices.Add(Conv(from) - normal);
        vertices.Add(Conv(from) + normal);
        vertices.Add(Conv(to) - normal);
        vertices.Add(Conv(to) + normal);

        // Colors (optional)
        colors.Add(col);
        colors.Add(col);
        colors.Add(col);
        colors.Add(col);

        // 2 triangles
        triangles.Add(vIndex + 0);
        triangles.Add(vIndex + 1);
        triangles.Add(vIndex + 2);

        triangles.Add(vIndex + 2);
        triangles.Add(vIndex + 1);
        triangles.Add(vIndex + 3);
    }

    public void ApplyMesh()
    {
        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetColors(colors);
        mesh.RecalculateNormals();
    }

    private Vector3 Conv(Vector2 v2)
    {
        return new Vector3(v2.x, v2.y, 0);
    }
}
