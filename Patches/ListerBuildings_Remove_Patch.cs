using Harmony;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(ListerBuildings), "Remove")]
    public class ListerBuildings_Remove_Patch
    {
        static void Postfix(ref Building b)
        {
            Main.Instance?.LabelPlacementHandler?.SetDirtyIfAreaIsOnMap(b.Map);
        }
    }
}