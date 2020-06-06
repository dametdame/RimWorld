using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using System.Reflection;


namespace DActiveCircuits.Core_Patches
{
    [HarmonyPatch(typeof(CompPower))]
    [HarmonyPatch("CompPrintForPowerGrid")]
    public static class Patch_CompPrintForPowerGrid_Prefix
    {
        public static bool Prefix(SectionLayer layer, CompPower __instance)
        {
            ActiveCircuitsBase.DrawPowerGrid(layer, __instance);
            return false;
        }
	}
}
