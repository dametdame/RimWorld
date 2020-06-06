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
    [HarmonyPatch(typeof(JobDriver_FoodDeliver))]
    [HarmonyPatch("MakeNewToils")]
    class Patch_FoodDeliver_Postfix
    {
        private static IEnumerable<Toil> Postfix(IEnumerable<Toil> values, JobDriver_FoodDeliver __instance)
        {
            FieldInfo efi = AccessTools.Field(typeof(JobDriver_FoodDeliver), "eatingFromInventory");
            bool eatingFromInventory = (bool)efi.GetValue(__instance);
            FieldInfo unpd = AccessTools.Field(typeof(JobDriver_FoodDeliver), "usingNutrientPasteDispenser");
            bool usingNutrientPasteDispenser = (bool)unpd.GetValue(__instance);
            int numToilsBeforeGoto;
            if (eatingFromInventory)
            {
                numToilsBeforeGoto = 1;
            }
            else if (usingNutrientPasteDispenser){
                numToilsBeforeGoto = 2;
            }
            else
            {
                numToilsBeforeGoto = 3;
            }
           
            foreach(Toil toil in HeatMealInjector.InjectHeat(values, __instance, numToilsBeforeGoto))
            {
                yield return toil;
            }
            yield break;
        }
    }
}
