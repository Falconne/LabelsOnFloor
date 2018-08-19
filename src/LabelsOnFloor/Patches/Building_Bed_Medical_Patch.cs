using Harmony;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Building_Bed))]
    [HarmonyPatch("Medical", PropertyMethod.Setter)]
    public class Building_Bed_Medical_Patch
    {
        [HarmonyPostfix]
        public static void ChangeMedicalStatus(Building_Bed __instance, ref bool value)
        {
            var room = __instance.GetRoom();
            if (room != null)
            {
                Main.Instance?.LabelPlacementHandler?.AddOrUpdateRoom(room);
            }
        }
    }
}