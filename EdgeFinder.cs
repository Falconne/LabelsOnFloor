using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public class EdgeFinder
    {
        private Map _map;

        public EdgeFinder(Map map)
        {
            _map = map;
        }

        public PlacementData GetBestPlacementData(
            IList<IntVec3> allCells,
            Func<IntVec3, bool> shouldBailout,
            Func<IntVec3, bool> isValidCell,
            int labelLength
            )
        {
            var lastRowCells = GetEdgeCells(
                allCells,
                shouldBailout,
                isValidCell,
                c => c.z
            );

            if (lastRowCells == null)
                return null;

            var lastColCells = GetEdgeCells(
                allCells,
                shouldBailout,
                isValidCell,
                c => c.x
            );

            if (lastColCells == null)
                return null;

            var placementData = new PlacementData();
            if (lastRowCells.Count < lastColCells.Count)
            {
                placementData.Position = GetFirstCellInColumn(lastColCells);
                placementData.Scale = GetScalingVector(lastColCells.Count, labelLength);
                placementData.Flipped = true;
            }
            else
            {
                placementData.Position = GetFirstCellInRow(lastRowCells);
                placementData.Scale = GetScalingVector(lastRowCells.Count, labelLength);
            }

            return placementData;
        }

        private static Vector3 GetScalingVector(int cellCount, int labelLength)
        {
            var scaling = (float)cellCount / labelLength;
            if (scaling > 1f)
                scaling = 1f;

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

        private static List<IntVec3> GetEdgeCells(
            IEnumerable<IntVec3> allCells,
            Func<IntVec3, bool> shouldBailout,
            Func<IntVec3, bool> isValidCell,
            Func<IntVec3, int> getIndexingDimensionValue
            )
        {
            var lastIndexFound = int.MaxValue;
            var result = new List<IntVec3>();
            foreach (var cell in allCells)
            {
                if (shouldBailout(cell))
                    return null;

                if (!isValidCell(cell))
                    continue;

                var indexingDimensionValue = getIndexingDimensionValue(cell);
                if (indexingDimensionValue < lastIndexFound)
                {
                    lastIndexFound = indexingDimensionValue;
                    result.Clear();
                }

                if (indexingDimensionValue == lastIndexFound)
                    result.Add(cell);

            }

            return result.Count == 0 ? null : result;
        }
    }
}