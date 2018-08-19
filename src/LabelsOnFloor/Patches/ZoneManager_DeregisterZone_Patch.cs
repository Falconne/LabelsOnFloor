using Harmony;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(ZoneManager), "DeregisterZone")]
    public class ZoneManager_DeregisterZone_Patch
    {
        static void Postfix(ref Zone oldZone)
        {
            Main.Instance?.LabelPlacementHandler?.SetDirtyIfAreaIsOnMap(oldZone.Map);
        }
    }
}