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
    [HarmonyPatch("TryGiveJob")]
    class Patch_OptimizeApparel_Prefix
    {

        public static bool Prefix(Pawn pawn, ref Job __result) // remove disallowed apparel
        {
            if (pawn.IsQuestLodger())
            {
                return true;
            }
            if (!DebugViewSettings.debugApparelOptimize)
            {
                if (Find.TickManager.TicksGame < pawn.mindState.nextApparelOptimizeTick)
                {
                    return true;
                }
            }
            List<ThingWithComps> equipment = pawn.equipment.AllEquipmentListForReading;
            foreach (ThingWithComps equip in equipment)
            {
                if (Base.IsResearchLocked(equip.def, pawn))
                {
                    Job job = JobMaker.MakeJob(JobDefOf.DropEquipment, equip);
                    __result = job;
                    return false;
                }
            }
            List<Apparel> wornApparel = pawn.apparel.WornApparel;
            for (int i = wornApparel.Count - 1; i >= 0; i--)
            {
                if (Base.IsResearchLocked(wornApparel[i].def, pawn))
                {
                    Job job = JobMaker.MakeJob(JobDefOf.RemoveApparel, wornApparel[i]);
                    job.haulDroppedApparel = true;
                    __result = job;
                    return false;
                }
            }

            return true;
        }

    }
}
