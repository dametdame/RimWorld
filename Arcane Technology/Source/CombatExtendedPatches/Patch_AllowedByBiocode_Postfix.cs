using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;
using AwesomeInventory;

namespace DArcaneTechnology
{
    class Patch_AllowedByBiocode_Postfix
    {
        
        private static void Postfix(Thing thing, Pawn pawn, ref bool __result) // disable float menu option if can't wear
        {
            __result = __result && !Base.IsResearchLocked(thing.def, pawn);
        }
        
    }
}
