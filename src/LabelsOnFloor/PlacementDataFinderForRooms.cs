using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    public class PlacementDataFinderForRooms
    {
        private readonly List<IntVec3> _blockedCells = new List<IntVec3>();

        private readonly Map _map;

        public PlacementDataFinderForRooms(Map map)
        {
            _map = map;
            foreach (var building in map.listerBuildings.allBuildingsColonist)
            {
                _blockedCells.AddRange(building.OccupiedRect().Cells);
            }
        }

        public PlacementData GetData(Room room, int labelLength)
        {
            return EdgeFinder.GetBestPlacementData(
                room.Cells.ToList(),
                c => false,
                c => !_map.thingGrid.CellContains(c, ThingDefOf.Wall),
                IsCellVisible,
                labelLength
            );
        }

        private bool IsCellVisible(IntVec3 cell)
        {
            return !_blockedCells.Any(c => c == cell);
        }
    }
}