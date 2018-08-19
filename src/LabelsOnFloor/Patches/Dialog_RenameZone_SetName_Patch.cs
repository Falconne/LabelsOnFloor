using Harmony;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Dialog_RenameZone), "SetName")]
    public class Dialog_RenameZone_SetName_Patch
    {
        static void Postfix()
        {
            Main.Instance?.LabelPlacementHandler?.SetDirty();
        }
    }
}