using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;
using AwesomeInventory;
using Verse.AI;


namespace DArcaneTechnology.ArmorRackPatches
{
    class Patch_CanStoreApparel_Postfix
    {

        private static void Postfix(Apparel apparel, ref bool __result)
        {
            __result = __result && !Base.IsResearchLocked(apparel.def);
        }

    }
}
