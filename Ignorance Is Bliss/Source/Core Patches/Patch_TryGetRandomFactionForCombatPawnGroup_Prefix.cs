using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIgnoranceIsBliss.Core_Patches;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DIgnoranceIsBliss
{
    [HarmonyPatch(typeof(PawnGroupMakerUtility))]
    [HarmonyPatch("TryGetRandomFactionForCombatPawnGroup")]
    class Patch_TryGetRandomFactionForCombatPawnGroup_Prefix
    {
        public static bool Prefix(ref Predicate<Faction> validator)
        {
            if(validator == null)
            {
                validator = (Faction f) => IgnoranceBase.FactionInEligibleTechRange(f);
            }
            return true;
        }
    }
}
