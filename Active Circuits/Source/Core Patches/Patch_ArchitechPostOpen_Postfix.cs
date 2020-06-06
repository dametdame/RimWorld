using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace DActiveCircuits.Core_Patches
{
    
    [HarmonyPatch(typeof(MainTabWindow_Architect))]
    [HarmonyPatch("PostOpen")]
    class Patch_ArchitechPostOpen_Postfix
    {
        public static void Postfix()
        {
           ActiveCircuitsBase.UpdateAllNets(Find.CurrentMap);
        }
    }
}
