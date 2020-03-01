using HarmonyLib;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(GatherSpotLister), "RegisterDeactivated")]
    public class GatherSpotLister_RegisterDeactivated_Patch
    {
        static void Postfix(ref CompGatherSpot spot)
        {
            Main.Instance?.LabelPlacementHandler?.SetDirty();
        }
    }
}