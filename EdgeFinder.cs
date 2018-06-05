using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public class EdgeFinder
    {
        public static PlacementData GetBestPlacementData(
            IEnumerable<IntVec3> allCells,
            Func<IntVec3, bool> shouldBailout,
            Func<IntVec3, bool> isValidCell,
            int labelLength
            )
        {
            var lastRowCellsRaw = GetEdgeCells(
                allCells,
                shouldBailout,
                isValidCell,
                c => c.z
            );

            if (lastRowCellsRaw == null)
                return null;

            var lastRowCells = lastRowCellsRaw.ToList();
            var scaling = (float)lastRowCells.Count / labelLength;
            if (scaling > 1f)
                scaling = 1f;
            lastRowCells.Sort((c1, c2) => c1.x.CompareTo(c2.x));

            return new PlacementData
            {
                Position = lastRowCells.First(),
                Scale = new Vector3(scaling, 1f, scaling)
            };

        }

        private static IEnumerable<IntVec3> GetEdgeCells(
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