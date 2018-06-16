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
            return RoomRoleFinder.GetRoomAtLocation(loc, Find.VisibleMap) != null;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            var map = Find.VisibleMap;
            var room = c.GetRoom(map);
            if (room == null)
                return;

            Find.WindowStack.Add(Main.Instance.GetRoomRenamer(room, c));
        }
    }
}