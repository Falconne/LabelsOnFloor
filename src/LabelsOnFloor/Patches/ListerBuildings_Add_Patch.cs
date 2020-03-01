using HarmonyLib;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(ListerBuildings), "Add")]
    public class ListerBuildings_Add_Patch
    {
        static void Postfix(ref Building b)
        {
            Main.Instance?.LabelPlacementHandler?.SetDirtyIfAreaIsOnMap(b.Map);
        }
    }
}