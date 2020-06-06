using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;
using AwesomeInventory;
using Verse.AI;

namespace DArcaneTechnology
{
    class Patch_BaseRackJob_Prefix
    {

        private static bool Prefix(JobDriver __instance, ref bool __result)
        {
            Thing item = __instance.job.GetTarget(TargetIndex.A).Thing;
            if (item != null && Base.IsResearchLocked(item.def))
            {
                __result = false;
                return false;
            }
            return true;
        }

    }
}
