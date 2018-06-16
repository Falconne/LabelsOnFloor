using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    public class RoomRoleFinder
    {
        private RoomRoleDef _emptyRooomRole;

        public bool IsImportantRoom(Room room)
        {
            if (room.Role == RoomRoleDefOf.None)
                return false;

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
    }
}