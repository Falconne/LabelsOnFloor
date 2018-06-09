using Harmony;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Room), "MakeNew")]
    public class Room_MakeNew_Patch
    {
        static void Postfix(ref Room __result)
        {
            Main.Instance?.LabelPlacementHandler?.AddRoom(__result, null);
        }

    }
}