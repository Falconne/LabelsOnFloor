using System;
using System.Collections.Generic;
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

        public IEnumerable<IntVec3> GetEdgeCells(
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