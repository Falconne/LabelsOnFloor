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
            return _customRoomLabelManager.IsRoomCustomised(room)
                ? _customRoomLabelManager.GetCustomLabelFor(room)
                : room.Role.LabelCap.ToUpper();
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