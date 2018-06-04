using System.Reflection;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    public class LabelMaker
    {
        public string GetRoomLabel(Room room)
        {
            return room.Role.LabelCap.ToUpper();
        }

        public string GetZoneLabel(Zone zone)
        {
            var growingZone = zone as Zone_Growing;
            if (growingZone == null)
                return zone.label.ToUpper();

            return growingZone.GetPlantDefToGrow().LabelCap.ToUpper();
        }
    }
}