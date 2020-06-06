using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using DHotMeals.Comps;

namespace DHotMeals.Core_Patches
{
    [HarmonyPatch(typeof(Toils_Ingest))]
    [HarmonyPatch("CarryIngestibleToChewSpot")]
    class Patch_CarryIngestibleToChewSpot_Postfix
    {
        public static void Postfix(Pawn pawn, TargetIndex ingestibleInd, Toil __result)
        {
            JobDriver_Ingest jdi = pawn.jobs.curDriver as JobDriver_Ingest;
            if (jdi == null)
                return;
            if (pawn.RaceProps.ToolUser)
            {
                LocalTargetInfo food = jdi.job.GetTarget(ingestibleInd);
                CompDFoodTemperature comp = food.Thing.TryGetComp<CompDFoodTemperature>();
                if (comp != null
                    && (comp.PropsTemp.likesHeat || (HotMealsSettings.thawIt && comp.curTemp < 0 && !comp.PropsTemp.okFrozen))   
                    )
                {
                    Patch_PrepareToIngestToils_ToolUser_Postfix.carryToils.Add(__result);
                }
            }       
        }
    }
}
