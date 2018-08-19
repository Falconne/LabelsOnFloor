using Harmony;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(DynamicDrawManager), "DrawDynamicThings")]
    public class DynamicDrawManager_DrawDynamicThings_Patch
    {
        static bool Prefix(ref DynamicDrawManager __instance)
        {
            Main.Instance?.Draw();

            return true;
        }
    }

}