using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace DArcaneTechnology.CorePatches
{
    [HarmonyPatch(typeof(JobGiver_OptimizeApparel))]
    [HarmonyPatch("ApparelScoreGain")]
    class Patch_ApparelScoreGain_Prefix
    {
        public static bool Prefix(Pawn pawn, Apparel ap, ref float __result) // keep pawns from automatically equipping disallowed stuff
        {
            if (Base.IsResearchLocked(ap.def, pawn))
            {
                __result = -5000f;
                return false;
            }
            return true;
        }

    }
}
