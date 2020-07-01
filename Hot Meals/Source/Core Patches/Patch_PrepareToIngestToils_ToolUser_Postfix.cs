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
    [HarmonyPatch(typeof(JobDriver_Ingest))]
    [HarmonyPatch("PrepareToIngestToils_ToolUser")]
    public static class Patch_PrepareToIngestToils_ToolUser_Postfix
    {
        public static List<Toil> carryToils = new List<Toil>();

        private static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_Ingest __instance, Toil chewToil)
        {
            LocalTargetInfo food = __instance.job.GetTarget(TargetIndex.A); ;
            foreach (Toil baseToil in values)
            {
                if(carryToils.Contains(baseToil) )
                {
                    carryToils.Remove(baseToil);           
                    foreach (Toil toil in HeatMealInjector.Heat(__instance))
                    {
                        yield return toil;
                    }
                }
                yield return baseToil;
            }

            if (food.Thing.def.IsDrug)
            {

                foreach (Toil toil in HeatMealInjector.Heat(__instance))
                {
                    yield return toil;
                }
                yield return Toils_Ingest.FindAdjacentEatSurface(TargetIndex.B, TargetIndex.A);
            }
            yield break;
        }
    }
}
