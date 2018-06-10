using Harmony;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(ZoneManager), "RegisterZone")]
    public class ZoneManager_RegisterZone_Patch
    {
        static void Postfix(ref Zone newZone)
        {
            Main.Instance?.LabelPlacementHandler?.SetDirtyIfAreaIsOnMap(newZone.Map);
        }
    }
}