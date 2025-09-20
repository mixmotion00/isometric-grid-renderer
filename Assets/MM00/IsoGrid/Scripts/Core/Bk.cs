//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Mixmotion00.Grid
//{
//    public struct Cross
//    {
//        public Cross(Vector2 pos, float size, Color col, Action<DrawLine> drawLine)
//        {
//            Pos = pos;
//            Size = size;
//            Col = col;
//            DrawLn = drawLine;
//        }

//        public Vector2 Pos { get; set; }
//        public float Size { get; set; }
//        public Color Col { get; set; }
//        public Action<DrawLine> DrawLn { get; set; }
//    }

//    public struct Square
//    {
//        public Vector2[] Points;
//        public Color Col;
//        public Action<DrawLine> DrawLn { get; set; }

//        public Square(Vector2[] points, Color col, Action<DrawLine> drawLine)
//        {
//            Points = points;
//            Col = col;
//            DrawLn = drawLine;
//        }
//    }

//    public struct DrawLine
//    {
//        public DrawLine(Vector2 from, Vector2 to, Color color)
//        {
//            From = from;
//            To = to;
//            Color = color;
//        }

//        public Vector2 From { get; set; }
//        public Vector2 To { get; set; }
//        public Color Color { get; set; }
//    }

//    public class GridLayout : MonoBehaviour
//    {
//        public float diamondW, diamondH;

//        private void OnDrawGizmos()
//        {
//            //DrawGrid(50, 50, 1, 0.1f);
//            Action<DrawLine> drawCb = (draw) =>
//            {

//                Debug.DrawLine(draw.From, draw.To, draw.Color);
//            };

//            DrawGrid(10, 10, diamondW, diamondH, DrawSquare, DrawCross, drawCb);
//        }

//        private void DrawGrid(int rows, int cols, float cellSizeW, float cellSizeH,
//            Action<Vector2[]> drawSquares, Action<Cross> drawCross,
//            Action<DrawLine> drawLine)
//        {
//            float gridWidth = cols * cellSizeW;
//            float gridHeight = rows * cellSizeH;

//            for (int i = 0; i < rows; i++)
//            {
//                for (int j = 0; j < cols; j++)
//                {
//                    // Position before centering
//                    Vector2 pos = new Vector2(j * cellSizeW, i * cellSizeH);

//                    // Shift so grid is centered at (0,0)
//                    pos.x -= gridWidth / 2f - cellSizeW / 2f;
//                    pos.y -= gridHeight / 2f - cellSizeH / 2f;

//                    // Build diamond
//                    var diamond = IsoDiamond(pos, cellSizeW, cellSizeH);

//                    // Draw outline
//                    //DrawSquare(diamond, Vector2.zero, Vector2.zero);
//                    drawSquares(diamond);

//                    //DrawCross(pos, 0.1f);
//                    drawCross(new Cross(pos, 0.1f, Color.red, drawLine));
//                    var offCross = pos;
//                    offCross.x = pos.x + cellSizeW / 2;
//                    offCross.y = pos.y + cellSizeH / 2;
//                    drawCross(new Cross(offCross, 0.1f, Color.red, drawLine));
//                    //DrawCross(offCross, 0.1f);
//                }
//            }
//        }


//        //private void DrawGrid(int rows, int cols, float cellSize, float crossSize)
//        //{
//        //    float gridWidth = cols * cellSize;
//        //    float gridHeight = rows * cellSize;

//        //    for (int i = 0; i < rows; i++)
//        //    {
//        //        for (int j = 0; j < cols; j++)
//        //        {
//        //            // Position before centering
//        //            Vector2 pos = new Vector2(j * cellSize, i * cellSize);

//        //            // Shift so grid is centered at (0,0)
//        //            pos.x -= gridWidth / 2f - cellSize / 2f;
//        //            pos.y -= gridHeight / 2f - cellSize / 2f;

//        //            // Draw a cross
//        //            DrawCross(pos, crossSize);

//        //            // Optional square (now also centered)
//        //            //var square = Square(new Vector2(pos.x + 0.1f, pos.y));
//        //            //square[0] += tlOffset;
//        //            //square[1] += trOffset;
//        //            //square[2] += bROffset;
//        //            //square[3] += blOffset;
//        //            //DrawSquare(square, topTilt, bottomTilt);

//        //            //draw iso diamond
//        //            var diamond = IsoDiamond(new Vector2(pos.x, pos.y), diamondW, diamondH);
//        //            DrawSquare(diamond, Vector2.zero, Vector2.zero);
//        //        }
//        //    }
//        //}

//        //private void DrawGrid(int rows, int cols, float cellSize, float crossSize)
//        //{
//        //    for (int i = 0; i < rows; i++)
//        //    {
//        //        for (int j = 0; j < cols; j++)
//        //        {
//        //            // Compute the position of this grid cell
//        //            Vector2 pos = new Vector2(j * cellSize, i * cellSize);

//        //            // Draw a cross at that position
//        //            DrawCross(pos, crossSize);
//        //            var square = Square(new Vector2(pos.x + 0.1f, pos.y));
//        //            DrawSquare(square, 0.5f, -0.5f);
//        //        }
//        //    }
//        //}

//        private void DrawCross(Cross cross)
//        {
//            DrawCross(cross.Pos, cross.Size, cross.Col, cross.DrawLn);
//        }

//        private void DrawCross(Vector2 pos, float size, Color col, Action<DrawLine> drawLine)
//        {
//            float half = size / 2f;

//            // First diagonal
//            var from = new Vector2(pos.x - half, pos.y + half);
//            var to = new Vector2(pos.x + half, pos.y - half);
//            var dl = new DrawLine(from, to, col);
//            drawLine.Invoke(dl);
//            //Debug.DrawLine(from, to, Color.red);

//            // Second diagonal
//            from = new Vector2(pos.x + half, pos.y + half);
//            to = new Vector2(pos.x - half, pos.y - half);
//            dl = new DrawLine(from, to, col);
//            drawLine.Invoke(dl);
//            //Debug.DrawLine(from, to, Color.red);
//        }

//        private Vector2[] IsoDiamond(Vector2 origin, float width, float height)
//        {
//            float halfW = width / 2f;
//            float halfH = height / 2f;

//            var top = new Vector2(origin.x, origin.y + halfH);
//            var right = new Vector2(origin.x + halfW, origin.y);
//            var bottom = new Vector2(origin.x, origin.y - halfH);
//            var left = new Vector2(origin.x - halfW, origin.y);

//            return new Vector2[] { top, right, bottom, left };
//        }


//        private Vector2[] Square(Vector2 origin, float wSize = 1f, float hSize = 1f)
//        {
//            float halfW = wSize / 2f;
//            float halfH = hSize / 2f;

//            var topLeft = new Vector2(origin.x - halfW, origin.y + halfH);
//            var topRight = new Vector2(origin.x + halfW, origin.y + halfH);
//            var bottomRight = new Vector2(origin.x + halfW, origin.y - halfH);
//            var bottomLeft = new Vector2(origin.x - halfW, origin.y - halfH);

//            return new Vector2[] { topLeft, topRight, bottomRight, bottomLeft };
//        }

//        private void DrawSquare(Vector2[] square)
//        {
//            DrawSquare(square, Vector2.zero, Vector2.zero);
//        }


//        private void DrawSquare(Vector2[] square, Vector2 topTilt, Vector2 bottomTilt)
//        {
//            var topLeft = Tilt(square[0], topTilt.x, topTilt.y);
//            var topRight = Tilt(square[1], topTilt.x, topTilt.y);
//            var bottomRight = Tilt(square[2], bottomTilt.x, bottomTilt.y);
//            var bottomLeft = Tilt(square[3], bottomTilt.x, bottomTilt.y);

//            var color = Color.green;

//            Debug.DrawLine
//                (
//                    topLeft, topRight, color
//                );

//            Debug.DrawLine
//                (
//                    topRight, bottomRight, color
//                );

//            Debug.DrawLine
//                (
//                    bottomRight, bottomLeft, color
//                );

//            Debug.DrawLine
//                (
//                    bottomLeft, topLeft, color
//                );
//        }

//        private Vector2 Tilt(Vector2 point, float xTilt, float yTilt)
//        {
//            return new Vector2(point.x + xTilt, point.y + yTilt);
//        }
//    }
//}
