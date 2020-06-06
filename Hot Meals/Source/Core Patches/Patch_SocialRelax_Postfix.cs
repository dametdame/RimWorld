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
using System.Reflection;

namespace DHotMeals.Core_Patches
{
    [HarmonyPatch(typeof(JobDriver_SocialRelax))]
    [HarmonyPatch("MakeNewToils")]
    class Patch_SocialRelax_Postfix
    {
        private static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_SocialRelax __instance)
        {

            if (!__instance.job.GetTarget(TargetIndex.C).HasThing)
            {
                foreach (Toil value in values)
                {
                    yield return value;
                }
                yield break;
            }

            int numToilsBeforeGoto = 2;
           
            foreach (Toil toil in HeatMealInjector.InjectHeat(values, __instance, numToilsBeforeGoto, foodIndex: TargetIndex.C, finalLocation: TargetIndex.B))
            {
                yield return toil;
            }
            yield break;
        }
    }
}