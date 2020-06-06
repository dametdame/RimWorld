using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DIgnoranceIsBliss.Core_Patches
{
    [HarmonyPatch(typeof(IncidentWorker_PawnsArrive))]
    [HarmonyPatch("FactionCanBeGroupSource")]
    class Patch_FactionCanBeGroupSource_Postfix
    {

        public static void Postfix(Faction f, ref bool __result)
        {
            __result = __result && IgnoranceBase.FactionInEligibleTechRange(f);
        }
    }
}
