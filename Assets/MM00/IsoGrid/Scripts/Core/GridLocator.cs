using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mixmotion00.Grid
{
    public struct GridObject
    {
        public GridObject(Vector2 pos, int size)
        {
            Pos = pos;
            Size = size;
        }

        public Vector2 Pos { get; set; }
        public int Size { get; set; }
    }

    public static class GridLocator
    {
        // #old vertical horizontal
        public static List<Vector2> GetIsoRangeHorVer(Vector2 center, int size, float cellW, float cellH)
        {
            var result = new List<Vector2> { center };

            size -= 1;

            // vertical & horizontal
            var startX = center.x - (cellW * size);
            var startY = center.y - (cellH * size);
            for (int i = 0; i <= size * 2; i++)
            {
                var update = center;
                update.x = startX;
                result.Add(update);
                startX += cellW;

                update = center;
                update.y = startY;
                result.Add(update);
                startY += cellH;
            }

            return result;
        }

        /// <summary>
        /// To get all surrounding cell's neighbors in diamond shape. 
        /// If size = 1, it will expand the isometric shape by 1. 
        /// Set size = 0 if no expansion. 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="cellW"></param>
        /// <param name="cellH"></param>
        /// <returns></returns>
        public static List<Vector2> GetIsoRange(Vector2 center, int size, float cellW, float cellH)
        {
            var result = new List<Vector2> { center };

            result.Clear();

            var diagRowCount = (size * 2) + 1;

            //var startY = center.y + (cellH * size);
            //var startX = center.x;
            //var start = new Vector2(startX, startY);

            //for (int j = 0; j < diagRowCount; j++)
            //{
            //    var update = start;
            //    result.Add(update);

            //    start.x -= cellW / 2;
            //    start.y -= cellH / 2;
            //}

            //startY = center.y + (cellH * size) - cellH/2;
            //startX = center.x + cellW / 2;
            //start = new Vector2(startX, startY);

            //for (int j = 0; j < diagRowCount; j++)
            //{
            //    var update = start;
            //    result.Add(update);

            //    start.x -= cellW / 2;
            //    start.y -= cellH / 2;
            //}

            for (int i = 0; i < diagRowCount; i++)
            {
                var startY = center.y + (cellH * size) - ((cellH / 2) * i);
                var startX = center.x + ((cellW / 2) * i);
                var start = new Vector2(startX, startY);

                for (int j = 0; j < diagRowCount; j++)
                {
                    var update = start;
                    result.Add(update);

                    start.x -= cellW / 2;
                    start.y -= cellH / 2;
                }
            }

            return result;
        }

        public static List<Vector2> GetIsoEdge(Vector2 center, int size, float cellW, float cellH) 
        {
            var isoRange = GetIsoRange(center, size, cellW, cellH);

            if(!isoRange.Any())
                return new List<Vector2>();

            var top = isoRange.First();
            var bottom = isoRange.First();
            //var left = isoRange.First();
            //var right = isoRange.First();

            foreach (var cell in isoRange) 
            {
                // get top
                if(cell.y > top.y)
                    top = cell;
                // get bottom
                if(cell.y < bottom.y)
                    bottom = cell;
                //// get left
                //if(cell.x < left.x)
                //    left = cell;
                //// get right
                //if(cell.x > right.x)
                //    right = cell;
            }

            // lines top right
            var diagonalTopRight = new List<Vector2>();
            Vector2 lastD = top;
            for (int i = 0; i < size*2; i++)
            {
                lastD.x += cellW / 2;
                lastD.y -= cellH / 2;

                diagonalTopRight.Add(lastD);
            }

            // lines top left
            var diagonalTopLeft = new List<Vector2>();
            lastD = top;
            for (int i = 0; i < size * 2; i++)
            {
                lastD.x -= cellW / 2;
                lastD.y -= cellH / 2;

                diagonalTopLeft.Add(lastD);
            }

            // lines bottom left
            var diagonalBottomLeft = new List<Vector2>();
            lastD = bottom;
            for (int i = 0; i < size * 2; i++)
            {
                lastD.x -= cellW / 2;
                lastD.y += cellH / 2;

                diagonalBottomLeft.Add(lastD);
            }

            // lines bottom right
            var diagonalBottomRight = new List<Vector2>();
            lastD = bottom;
            for (int i = 0; i < size * 2; i++)
            {
                lastD.x += cellW / 2;
                lastD.y += cellH / 2;

                diagonalBottomRight.Add(lastD);
            }


            var unify = new List<Vector2> { top, bottom };
            unify.AddRange(diagonalTopRight);
            unify.AddRange(diagonalTopLeft);
            unify.AddRange(diagonalBottomLeft);
            unify.AddRange(diagonalBottomRight);

            return unify;
        }

        public static bool IsOverlap(GridObject a, GridObject b, float cellW, float cellH, out List<Vector2> overlapPoints)
        {
            var rangeA = GetIsoRange(a.Pos, a.Size, cellW, cellH).ToHashSet();
            var rangeB = GetIsoRange(b.Pos, b.Size, cellW, cellH).ToHashSet();

            var overlaps = rangeB.Where(p => rangeA.Contains(p)).ToList();

            overlapPoints = overlaps;

            return overlaps.Any();
        }
    }
}
