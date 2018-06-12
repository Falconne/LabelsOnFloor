using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    internal class BestEdges
    {
        public readonly List<IntVec3> Row;
        public readonly List<IntVec3> Column;

        public BestEdges()
        {
            Row = new List<IntVec3>();
            Column = new List<IntVec3>();
        }

        public bool IsValid()
        {
            return Row.Count > 0 && Column.Count > 0;
        }
    }

    public class EdgeFinder
    {
        public static PlacementData GetBestPlacementData(
            IList<IntVec3> allCells,
            Func<IntVec3, bool> shouldBailout,
            Func<IntVec3, bool> isValidCell,
            Func<IntVec3, bool> isVisibleCell,
            int labelLength
            )
        {
            if (labelLength == 0)
                return null;

            var bestEdges = GetEdgeCells(
                allCells,
                shouldBailout,
                isValidCell
            );

            if (bestEdges == null)
                return null;

            var placementData = new PlacementData();
            var visibleCellsInRow = bestEdges.Row.Count(isVisibleCell);
            var visibleCellsInCol = bestEdges.Column.Count(isVisibleCell);
            if (visibleCellsInRow < visibleCellsInCol)
            {
                placementData.Position = GetFirstCellInColumn(bestEdges.Column);
                placementData.Scale = GetScalingVector(bestEdges.Column.Count, labelLength);
                placementData.Flipped = true;
            }
            else
            {
                placementData.Position = GetFirstCellInRow(bestEdges.Row);
                placementData.Scale = GetScalingVector(bestEdges.Row.Count, labelLength);
            }

            if (placementData.Scale.x < Main.Instance.GetMinFontScale())
                return null;

            return placementData;
        }

        private static Vector3 GetScalingVector(int cellCount, int labelLength)
        {
            var scaling = (cellCount - 0.4f) / labelLength;

            if (scaling > Main.Instance.GetMaxFontScale())
                scaling = Main.Instance.GetMaxFontScale();

            return new Vector3(scaling, 1f, scaling);

        }

        private static IntVec3 GetFirstCellInRow(IList<IntVec3> cells)
        {
            var bestCellFound = cells.First();
            foreach (var cell in cells)
            {
                if (cell.x >= bestCellFound.x)
                    continue;

                bestCellFound = cell;
            }

            return bestCellFound;
        }

        private static IntVec3 GetFirstCellInColumn(IList<IntVec3> cells)
        {
            var bestCellFound = cells.First();
            foreach (var cell in cells)
            {
                if (cell.z <= bestCellFound.z)
                    continue;

                bestCellFound = cell;
            }

            return bestCellFound;
        }

        private static BestEdges GetEdgeCells(
            IEnumerable<IntVec3> allCells,
            Func<IntVec3, bool> shouldBailout,
            Func<IntVec3, bool> isValidCell
            )
        {
            var result = new BestEdges();
            var lastRowIndexFound = int.MaxValue;
            var lastColIndexFound = int.MaxValue;
            foreach (var cell in allCells)
            {
                if (shouldBailout(cell))
                    return null;

                if (!isValidCell(cell))
                    continue;


                if (cell.z < lastRowIndexFound)
                {
                    lastRowIndexFound = cell.z;
                    result.Row.Clear();
                }

                if (cell.z == lastRowIndexFound)
                    result.Row.Add(cell);


                if (cell.x < lastColIndexFound)
                {
                    lastColIndexFound = cell.x;
                    result.Column.Clear();
                }

                if (cell.x == lastColIndexFound)
                    result.Column.Add(cell);
            }

            return result.IsValid() ? result : null;
        }
    }
}