using Verse;

namespace LabelsOnFloor
{
    public class PlacementDataFinderForZones
    {
        public static PlacementData GetData(Zone zone, Map map, int labelLength)
        {
            return EdgeFinder.GetBestPlacementData(
                zone.Cells,
                c => c.Fogged(map),
                c => true,
                c => true,
                labelLength
            );
        }
    }
}