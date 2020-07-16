using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace DIgnoranceIsBliss.Core_Patches
{
    [HarmonyPatch(typeof(QuestScriptDef))]
    [HarmonyPatch("CanRun")]
    [HarmonyPatch(new Type[] { typeof(Slate)})]
    class Patch_CanRun_Prefix
    {
        public static bool Prefix(ref bool __result, QuestScriptDef __instance)
        {
            TechLevel questTech;
            if (IgnoranceSettings.changeQuests && IgnoranceBase.questScriptDefs.TryGetValue(__instance.defName, out questTech))
            {
                if (!IgnoranceBase.TechIsEligibleForIncident(questTech))
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}
