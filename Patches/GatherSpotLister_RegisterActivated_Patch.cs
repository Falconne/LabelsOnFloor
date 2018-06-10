using Harmony;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(GatherSpotLister), "RegisterActivated")]
    public class GatherSpotLister_RegisterActivated_Patch
    {
        static void Postfix(ref CompGatherSpot spot)
        {
            Main.Instance?.LabelPlacementHandler?.SetDirty();
        }
    }
}