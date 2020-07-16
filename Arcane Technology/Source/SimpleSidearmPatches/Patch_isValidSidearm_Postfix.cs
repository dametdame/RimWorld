using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;
using SimpleSidearms.rimworld;

namespace DArcaneTechnology.SimpleSidearmPatches
{
    class Patch_isValidSidearm_Postfix
    {
        private static void Postfix(ref object sidearm, ref bool __result, ref string errString)
        {
            if (__result) {
                try
                {
                    ThingDef sa = (ThingDef)sidearm; // this cast seems to work, as the underlying struct (ThingDefStuffDefPair) defines a ThingDef first
                    if (Base.IsResearchLocked(sa))
                    {
                        __result = false;
                        errString = "DUnknownTechnology".Translate();
                    }
                }
                catch (Exception e)
                {
                    Log.ErrorOnce("Error in Arcane Technology simple sidearms postfix, exception is " + e, 6969420);
                }
            }
        }
    }
}
