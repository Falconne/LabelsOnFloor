using System.Reflection;
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
    }
}