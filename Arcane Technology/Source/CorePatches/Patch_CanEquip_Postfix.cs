using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;
using System.Reflection;

namespace DArcaneTechnology.CorePatches
{
    class Patch_CanEquip_Postfix
    {

        private static void Postfix(Thing thing, Pawn pawn, ref string cantReason, ref bool __result)
        {
            if (__result == true) 
            {
                if (Base.IsResearchLocked(thing.def, pawn))
                {
                    __result = false;
                    cantReason = "DUnknownTechnology".Translate();
                }
            }
        }

    }
}
