using RimWorld;
using UnityEngine;
using Verse;

namespace LabelsOnFloor
{
    public class Designator_Rename : Designator
    {
        public Designator_Rename()
        {
            defaultLabel = "Rename";
            icon = Resources.Rename;
            useMouseIcon = true;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            var map = Find.VisibleMap;
            return RoomRoleFinder.GetRoomAtLocation(loc, map) != null 
                || loc.GetZone(map) != null;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            var map = Find.VisibleMap;

            var zone = c.GetZone(map);
            if (zone != null)
            {
                Find.WindowStack.Add(new Dialog_RenameZone(zone));
                return;
            }

            var room = c.GetRoom(map);
            if (room != null)
                Find.WindowStack.Add(Main.Instance.GetRoomRenamer(room, c));
        }
    }
}