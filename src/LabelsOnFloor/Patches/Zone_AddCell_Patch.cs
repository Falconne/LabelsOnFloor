using Harmony;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Zone), "AddCell")]
    public class Zone_AddCell_Patch
    {
        static void Postfix(ref Zone __instance)
        {
            Main.Instance?.LabelPlacementHandler?.AddOrUpdateZone(__instance);
        }

    }
}