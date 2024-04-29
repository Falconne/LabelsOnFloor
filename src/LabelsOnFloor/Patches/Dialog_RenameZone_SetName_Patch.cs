using HarmonyLib;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(Dialog_RenameZone), "OnRenamed")]
    public class Dialog_RenameZone_SetName_Patch
    {
        static void Postfix()
        {
            Main.Instance?.LabelPlacementHandler?.SetDirty();
        }
    }
}