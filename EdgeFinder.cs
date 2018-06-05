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


            List<IntVec3> rangeToUse;
            var flipped = false;
            IntVec3 posCell;
            if (lastRowCells.Count < lastColCells.Count)
            {
                rangeToUse = lastColCells;
                posCell = GetFirstCell(rangeToUse, c => c.z);
                flipped = true;
            }
            else
            {
                rangeToUse = lastRowCells;
                posCell = GetFirstCell(rangeToUse, c => c.x);
            }

            var scaling = (float)rangeToUse.Count / labelLength;
            if (scaling > 1f)
                scaling = 1f;

            return new PlacementData
            {
                Position = posCell,
                Scale = new Vector3(scaling, 1f, scaling),
                Flipped = flipped
            };

        }

        private static IntVec3 GetFirstCell(IEnumerable<IntVec3> cells, Func<IntVec3, int> getValue)
        {
            var lowestValFound = int.MaxValue;
            IntVec3 bestCellFound = default;
            foreach (var cell in cells)
            {
                var val = getValue(cell);
                if (val >= lowestValFound)
                    continue;

                lowestValFound = val;
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

            return result.Count == 0 ? null: result;
        }
    }
}