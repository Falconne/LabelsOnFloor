using HarmonyLib;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Room), "Notify_RoomShapeOrContainedBedsChanged")]
    public class Room_Notify_RoomShapeOrContainedBedsChanged_Patch
    {
        static void Postfix(ref Room __instance)
        {
            Main.Instance?.LabelPlacementHandler?.SetDirtyIfAreaIsOnMap(__instance.Map);
        }
    }
}