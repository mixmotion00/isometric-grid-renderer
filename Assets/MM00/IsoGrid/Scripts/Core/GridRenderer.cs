using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

namespace Mixmotion00.Grid
{
    public struct Cross
    {
        public Cross(Vector2 pos, float size, Color col)
        {
            Pos = pos;
            Size = size;
            Col = col;
        }

        public Vector2 Pos { get; set; }
        public float Size { get; set; }
        public Color Col { get; set; }
    }

    public struct Square
    {
        public Vector2[] Points;
        public Color Col;

        public Square(Vector2[] points, Color col)
        {
            Points = points;
            Col = col;
        }

        public Vector2 Center
        {
            get
            {
                if (Points == null || Points.Length == 0)
                    return Vector2.zero;

                Vector2 sum = Vector2.zero;
                for (int i = 0; i < Points.Length; i++)
                {
                    sum += Points[i];
                }
                return sum / Points.Length;
            }
        }
    }

    public struct DrawLimit
    {
        public DrawLimit(Vector2 center, float wSize, float hSize)
        {
            Center = center;
            WSize = wSize;
            HSize = hSize;
        }

        public Vector2 Center { get; set; }
        public float WSize { get; set; }
        public float HSize { get; set; }

        public static DrawLimit Default()
        {
            return new DrawLimit(Vector2.zero, 1, 1);
        }

        public Vector2 RightTopLimit()
        {
            var l = Vector2.zero;
            l.x = Center.x + (1 * WSize);
            l.y = Center.y + (1 * HSize);

            return l;
        }

        public Vector2 BottomLeftLimit()
        {
            var l = Vector2.zero;
            l.x = Center.x + (-1 * WSize);
            l.y = Center.y + (-1 * HSize);

            return l;
        }
    }

    public struct DrawLine
    {
        public DrawLine(Vector2 from, Vector2 to, Color color)
        {
            From = from;
            To = to;
            Color = color;
        }

        public Vector2 From { get; set; }
        public Vector2 To { get; set; }
        public Color Color { get; set; }
    }

    public class GridRenderer
    {
        // *** For Debugging ***
        //public float diamondW, diamondH;

        //private void OnDrawGizmos()
        //{
        //    //DrawGrid(50, 50, 1, 0.1f);
        //    Action<DrawLine> drawCb = (draw) =>
        //    {

        //        Debug.DrawLine(draw.From, draw.To, draw.Color);
        //    };

        //    DrawGrid(
        //        10, 10, diamondW, diamondH,
        //        drawCb,
        //        Color.white, Color.red);
        //}

        public static Vector2 GetSnappedPosition(Vector3 worldPos, int rows, int cols, float cellW, float cellH)
        {
            // shift into grid space centered at (0,0)
            float gridWidth = cols * cellW;
            float gridHeight = rows * cellH;

            Vector3 local = worldPos;
            local.x += gridWidth / 2f - cellW / 2f;
            local.y += gridHeight / 2f - cellH / 2f;

            // compute indices for base grid
            int j = Mathf.RoundToInt(local.x / cellW);
            int i = Mathf.RoundToInt(local.y / cellH);

            Vector3 oriCross = new Vector3(j * cellW, i * cellH, worldPos.z);
            oriCross.x -= gridWidth / 2f - cellW / 2f;
            oriCross.y -= gridHeight / 2f - cellH / 2f;

            // compute indices for offset grid
            int j2 = Mathf.RoundToInt((local.x - cellW / 2f) / cellW);
            int i2 = Mathf.RoundToInt((local.y - cellH / 2f) / cellH);

            Vector3 offCross = new Vector3(j2 * cellW + cellW / 2f, i2 * cellH + cellH / 2f, worldPos.z);
            offCross.x -= gridWidth / 2f - cellW / 2f;
            offCross.y -= gridHeight / 2f - cellH / 2f;

            // pick nearest
            float dOri = Vector2.SqrMagnitude(worldPos - oriCross);
            float dOff = Vector2.SqrMagnitude(worldPos - offCross);

            return (dOri <= dOff) ? oriCross : offCross;
        }

        public static void DrawGrid(int rows, int cols, float cellSizeW, float cellSizeH,
            Action<DrawLine> drawLine,
            Color squareCol, Color crossCol,
            out List<Cross> visibleCrossess,
            DrawLimit? drawLimit = null)
        {
            ExecuteDrawGrid(rows, cols, cellSizeW, cellSizeH,
                DrawSquare, DrawCross,
                drawLine,
                squareCol, crossCol,
                out visibleCrossess,
                drawLimit);
        }

        private static void ExecuteDrawGrid(int rows, int cols, float cellSizeW, float cellSizeH,
            Action<Square, Action<DrawLine>> drawSquares, Action<Cross, Action<DrawLine>> drawCross,
            Action<DrawLine> drawLine,
            Color squareCol, Color crossCol,
            out List<Cross> visibleCrossess,
            DrawLimit? drawLimit = null)
        {
            float gridWidth = cols * cellSizeW;
            float gridHeight = rows * cellSizeH;

            List<Square> diamonds = new List<Square>();
            List<Cross> crosses = new List<Cross>();

            int minRow = 0;
            int maxRow = rows;
            int minCol = 0;
            int maxCol = cols;

            if (drawLimit.HasValue)
            {
                var limit = drawLimit.Value;

                minCol = Mathf.Max(0, Mathf.FloorToInt((limit.Center.x - limit.WSize / 2) / cellSizeW) + cols / 2);
                maxCol = Mathf.Min(cols - 1, Mathf.CeilToInt((limit.Center.x + limit.WSize / 2) / cellSizeW) + cols / 2);

                minRow = Mathf.Max(0, Mathf.FloorToInt((limit.Center.y - limit.HSize / 2) / cellSizeH) + rows / 2);
                maxRow = Mathf.Min(rows - 1, Mathf.CeilToInt((limit.Center.y + limit.HSize / 2) / cellSizeH) + rows / 2);
            }

            for (int i = minRow; i < maxRow; i++)
            {
                for (int j = minCol; j < maxCol; j++)
                {
                    // Position before centering
                    Vector2 pos = new Vector2(j * cellSizeW, i * cellSizeH);

                    // Shift so grid is centered at (0,0)
                    pos.x -= gridWidth / 2f - cellSizeW / 2f;
                    pos.y -= gridHeight / 2f - cellSizeH / 2f;

                    // Build diamond
                    var diamond = IsoDiamond(pos, cellSizeW, cellSizeH, squareCol);
                    if (drawLimit.HasValue && WithinRange(drawLimit.Value, pos))
                    {
                        diamonds.Add(diamond);
                    }

                    // Draw outline
                    //drawSquares(diamond, drawLine);

                    // X _ X _ X
                    // _ X _ X _
                    // this only draw X
                    //drawCross(new Cross(pos, 0.1f, crossCol), drawLine);
                    //crosses.Add(new Cross(pos, 0.1f, crossCol));
                    var oriCross = new Cross(pos, 0.1f, crossCol);

                    // X Z X Z X
                    // Z X Z X Z
                    // this draw Z
                    var offCrossPos = pos;
                    offCrossPos.x = pos.x + cellSizeW / 2;
                    offCrossPos.y = pos.y + cellSizeH / 2;
                    //drawCross(new Cross(offCross, 0.1f, crossCol), drawLine); //draw grid between interval
                    //crosses.Add(new Cross(offCrossPos, 0.1f, crossCol));
                    var offCross = new Cross(offCrossPos, 0.1f, crossCol);

                    if (drawLimit.HasValue && WithinRange(drawLimit.Value, oriCross.Pos))
                    {
                        // X _ X _ X
                        // _ X _ X _
                        // this only draw X
                        //drawCross(new Cross(pos, 0.1f, crossCol), drawLine);
                        crosses.Add(oriCross);
                    }

                    if (drawLimit.HasValue && WithinRange(drawLimit.Value, offCross.Pos))
                    {
                        // X Z X Z X
                        // Z X Z X Z
                        // this draw Z
                        crosses.Add(offCross); //draw grid between interval
                    }
                }
            }

            foreach (var diamond in diamonds)
            {
                drawSquares(diamond, drawLine);
            }

            foreach (var cross in crosses)
            {
                drawCross(cross, drawLine);
            }

            visibleCrossess = crosses;
        }

        private static bool WithinRange(DrawLimit drawLimit, Vector2 pos)
        {
            if (pos.x > drawLimit.RightTopLimit().x)
                return false;
            if (pos.y > drawLimit.RightTopLimit().y)
                return false;
            if (pos.x < drawLimit.BottomLeftLimit().x)
                return false;
            if (pos.y < drawLimit.BottomLeftLimit().y)
                return false;

            return true;
        }

        public static void DrawCross(Cross cross, Action<DrawLine> drawLine)
        {
            var pos = cross.Pos;
            var color = cross.Col;
            var size = cross.Size;
            DrawCross(pos, size, color, drawLine);
        }

        private static void DrawCross(Vector2 pos, float size, Color col, Action<DrawLine> drawLine)
        {
            float half = size / 2f;

            // First diagonal
            var from = new Vector2(pos.x - half, pos.y + half);
            var to = new Vector2(pos.x + half, pos.y - half);
            var dl = new DrawLine(from, to, col);
            drawLine.Invoke(dl);
            //Debug.DrawLine(from, to, Color.red);

            // Second diagonal
            from = new Vector2(pos.x + half, pos.y + half);
            to = new Vector2(pos.x - half, pos.y - half);
            dl = new DrawLine(from, to, col);
            drawLine.Invoke(dl);
            //Debug.DrawLine(from, to, Color.red);
        }

        private static Square IsoDiamond(Vector2 origin, float width, float height, Color color)
        {
            float halfW = width / 2f;
            float halfH = height / 2f;

            var top = new Vector2(origin.x, origin.y + halfH);
            var right = new Vector2(origin.x + halfW, origin.y);
            var bottom = new Vector2(origin.x, origin.y - halfH);
            var left = new Vector2(origin.x - halfW, origin.y);

            var shapePts = new Vector2[] { top, right, bottom, left };

            return new Square(shapePts, color);
        }


        private static Vector2[] Square(Vector2 origin, float wSize = 1f, float hSize = 1f)
        {
            float halfW = wSize / 2f;
            float halfH = hSize / 2f;

            var topLeft = new Vector2(origin.x - halfW, origin.y + halfH);
            var topRight = new Vector2(origin.x + halfW, origin.y + halfH);
            var bottomRight = new Vector2(origin.x + halfW, origin.y - halfH);
            var bottomLeft = new Vector2(origin.x - halfW, origin.y - halfH);

            return new Vector2[] { topLeft, topRight, bottomRight, bottomLeft };
        }

        private static void DrawSquare(Square square, Action<DrawLine> drawLine)
        {
            var shapePts = square.Points;
            var shapeCol = square.Col;
            DrawSquare(shapePts, shapeCol, Vector2.zero, Vector2.zero, drawLine);
        }

        private static void DrawSquare(Vector2[] square, Color color, Vector2 topTilt, Vector2 bottomTilt, Action<DrawLine> drawLine)
        {
            var topLeft = Tilt(square[0], topTilt.x, topTilt.y);
            var topRight = Tilt(square[1], topTilt.x, topTilt.y);
            var bottomRight = Tilt(square[2], bottomTilt.x, bottomTilt.y);
            var bottomLeft = Tilt(square[3], bottomTilt.x, bottomTilt.y);

            var line_1 = new DrawLine(topLeft, topRight, color);
            drawLine.Invoke(line_1);

            var line_2 = new DrawLine(topRight, bottomRight, color);
            drawLine.Invoke(line_2);

            var line_3 = new DrawLine(bottomRight, bottomLeft, color);
            drawLine.Invoke(line_3);

            var line_4 = new DrawLine(bottomLeft, topLeft, color);
            drawLine.Invoke(line_4);
        }

        private static Vector2 Tilt(Vector2 point, float xTilt, float yTilt)
        {
            return new Vector2(point.x + xTilt, point.y + yTilt);
        }
    }
}
