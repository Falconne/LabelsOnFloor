using System.Reflection;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    public class LabelMaker
    {
        private string _defaultGrowingZonePrefix;

        public LabelMaker()
        {
            _defaultGrowingZonePrefix = "GrowingZone".Translate();
        }

        public string GetRoomLabel(Room room)
        {
            return room.Role.LabelCap.ToUpper();
        }

        public string GetZoneLabel(Zone zone)
        {
            if (!(zone is Zone_Growing growingZone))
                return zone.label.ToUpper();

            // Use custom zone name, if it looks like it has been changed
            if (growingZone.label.StartsWith(_defaultGrowingZonePrefix))
                return growingZone.GetPlantDefToGrow().LabelCap.ToUpper();

            return growingZone.label.ToUpper();
        }
    }
}