using Harmony;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Building_Bed), "FacilityChanged")]
    public class Building_Bed_Medical_Patch
    {
        public static void Postfix(ref Building_Bed __instance)
        {
            var room = __instance.GetRoom();
            if (room != null)
            {
                Main.Instance?.LabelPlacementHandler?.AddOrUpdateRoom(room);
            }
        }
    }
}