using System.Reflection;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    public class LabelMaker
    {
        private readonly MethodInfo _roomLabelGetter;

        public LabelMaker()
        {
            var envInspector = GenTypes.GetTypeInAnyAssembly("EnvironmentInspectDrawer");

            _roomLabelGetter =
                envInspector?.GetMethod("GetRoomRoleLabel", BindingFlags.Static | BindingFlags.NonPublic);
        }

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