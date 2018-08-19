using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    public class RoomRoleFinder
    {
        private RoomRoleDef _emptyRooomRole;

        private readonly CustomRoomLabelManager _customRoomLabelManager;

        public RoomRoleFinder(CustomRoomLabelManager customRoomLabelManager)
        {
            _customRoomLabelManager = customRoomLabelManager;
        }

        public bool IsImportantRoom(Room room)
        {
            if (room.Role == RoomRoleDefOf.None)
                return false;

            if (_customRoomLabelManager.IsRoomCustomised(room))
                return true;

            if (_emptyRooomRole != null)
            {
                return room.Role != _emptyRooomRole;
            }

            if (room.Role.defName == "Room")
            {
                _emptyRooomRole = room.Role;
                return false;
            }

            return true;
        }

        public static Room GetRoomAtLocation(IntVec3 loc, Map map)
        {
            if (map == null)
                return null;

            var room = loc.GetRoom(map);
            if (room == null)
                return null;

            return room.Role == RoomRoleDefOf.None ? null : room;
        }
    }
}