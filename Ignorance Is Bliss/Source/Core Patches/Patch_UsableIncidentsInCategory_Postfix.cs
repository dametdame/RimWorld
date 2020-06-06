using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DIgnoranceIsBliss.Core_Patches
{
    [HarmonyPatch(typeof(StorytellerComp))]
    [HarmonyPatch("UsableIncidentsInCategory")]
    [HarmonyPatch(new Type[] {typeof(IncidentCategoryDef), typeof(Func<IncidentDef, IncidentParms>) })]
    class Patch_UsableIncidentsInCategory_Postfix
    {

        public static void Postfix(ref IEnumerable<IncidentDef> __result)
        {
            __result = from x in __result
                       where (!IgnoranceBase.incidentWorkers.ContainsKey(x.workerClass) || IgnoranceBase.TechIsEligibleForIncident(IgnoranceBase.incidentWorkers.TryGetValue(x.workerClass)))
                            && (!IgnoranceBase.incidentDefNames.ContainsKey(x.defName) || IgnoranceBase.TechIsEligibleForIncident(IgnoranceBase.incidentDefNames.TryGetValue(x.defName)))
                       select x;
        }
    }
}
