using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace LabelsOnFloor
{
    [HarmonyPatch(typeof(DesignationCategoryDef))]
    [HarmonyPatch("ResolvedAllowedDesignators", MethodType.Getter)]
    public class DesignationCategoryDef_ResolvedAllowedDesignators_Patch
    {
        private static DesignationCategoryDef ordersCategoryDef;

        static void Postfix(ref IEnumerable<Designator> __result, ref DesignationCategoryDef __instance)
        {
            if (!Main.Instance.IsModAcitve())
                return;

            if (ordersCategoryDef == null)
            {
                if ("Orders" == __instance.defName)
                    ordersCategoryDef = __instance;
            }

            if (__instance != ordersCategoryDef || __result == null)
                return;

            var ourGizmo = new Designator_Rename();

            var newList = __result.ToList();
            newList.Add(ourGizmo);
            __result = newList;
        }
    }
}