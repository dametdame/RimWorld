using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace DActiveCircuits.Core_Patches
{
    [HarmonyPatch(typeof(PowerNetManager))]
    [HarmonyPatch("PowerNetsTick")]
    class Patch_PowerNetsTick_Postfix
    {
        public static void Postfix(PowerNetManager __instance)
        {
            if (Find.CurrentMap == __instance.map && (OverlayDrawHandler.ShouldDrawPowerGrid || ActiveCircuitsSettings.displayBadOnMain) && GenTicks.TicksAbs % 149 == 0)
            {
                ActiveCircuitsBase.UpdateAllNets(__instance.map);
            }
        }
    }
}
