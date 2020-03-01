using HarmonyLib;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Room), "RemoveRegion")]
    public class Room_RemoveRegion_Patch
    {
        static void Postfix(ref Room __instance)
        {
            Main.Instance?.LabelPlacementHandler?.SetDirtyIfAreaIsOnMap(__instance.Map);
        }
    }
}