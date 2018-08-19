using Harmony;
using RimWorld;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Zone_Growing), "SetPlantDefToGrow")]
    public class Zone_Growing_SetPlantDefToGrow_Patch
    {
        static void Postfix(ref Zone_Growing __instance)
        {
            Main.Instance?.LabelPlacementHandler?.AddOrUpdateZone(__instance);
        }
      
    }
}