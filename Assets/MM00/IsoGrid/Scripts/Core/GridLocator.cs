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
