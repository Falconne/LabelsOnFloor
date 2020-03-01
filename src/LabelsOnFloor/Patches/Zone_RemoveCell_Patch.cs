using HarmonyLib;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Zone), "RemoveCell")]
    public class Zone_RemoveCell_Patch
    {
        static void Postfix(ref Zone __instance)
        {
            Main.Instance?.LabelPlacementHandler?.AddOrUpdateZone(__instance);
        }
    }
}