using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    public class LabelMaker
    {
        private readonly string _defaultGrowingZonePrefix;

        private readonly CustomRoomLabelManager _customRoomLabelManager;

        public LabelMaker(CustomRoomLabelManager customRoomLabelManager)
        {
            _customRoomLabelManager = customRoomLabelManager;
            _defaultGrowingZonePrefix = "GrowingZone".Translate();
        }

        public string GetRoomLabel(Room room)
        {
            if (room == null)
                return string.Empty;

            return _customRoomLabelManager.IsRoomCustomised(room)
                ? _customRoomLabelManager.GetCustomLabelFor(room)
                : room.Role.label.ToUpper();
        }

        public string GetZoneLabel(Zone zone)
        {
            if (zone == null)
                return string.Empty;

            if (!(zone is Zone_Growing growingZone))
                return zone.label?.ToUpper() ?? string.Empty;

            // Use custom zone name, if it looks like it has been changed
            if (growingZone.label?.StartsWith(_defaultGrowingZonePrefix) ?? false)
                return growingZone.GetPlantDefToGrow()?.label?.ToUpper() ?? string.Empty;

            return growingZone.label?.ToUpper() ?? string.Empty;
        }
    }
}