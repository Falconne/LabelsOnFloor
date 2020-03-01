using HarmonyLib;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Room), "MakeNew")]
    public class Room_MakeNew_Patch
    {
        static void Postfix(ref Room __result)
        {
            Main.Instance?.LabelPlacementHandler?.AddOrUpdateRoom(__result);
        }

    }
}