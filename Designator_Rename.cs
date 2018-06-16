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
            var room = loc.GetRoom(Find.VisibleMap);
            if (room == null)
                return false;

            if (room.Role == RoomRoleDefOf.None)
                return false;

            return true;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            Find.WindowStack.Add(new Dialog_RenameRoom());
        }
    }
}