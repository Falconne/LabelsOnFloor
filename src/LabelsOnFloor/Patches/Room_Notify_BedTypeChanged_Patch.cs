using Harmony;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Room), "Notify_BedTypeChanged")]
    public class Room_Notify_BedTypeChanged_Patch
    {
        static void Postfix(ref Room __instance)
        {
            Main.Instance?.LabelPlacementHandler?.AddOrUpdateRoom(__instance);
        }
    }
}