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
    [HarmonyPatch(typeof(JobDriver_FoodFeedPatient))]
    [HarmonyPatch("MakeNewToils")]
    public static class Patch_FoodFeedPatient_Postfix
    {
        private static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_FoodFeedPatient __instance)
        {
            int numToilsBeforeGoto = 2;
            if (__instance.pawn.inventory != null && __instance.pawn.inventory.Contains(__instance.job.targetA.Thing))
            {
                numToilsBeforeGoto--;
            }
            foreach (Toil toil in HeatMealInjector.InjectHeat(values, __instance, numToilsBeforeGoto))
            {
                yield return toil;
            }
            yield break;
        }
    }
}
