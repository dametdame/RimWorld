using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RimWorld;
using HarmonyLib;
using Verse;
using UnityEngine;

namespace DTimeControl.Core_Patches
{
    [HarmonyPatch(typeof(ThingWithComps))]
    [HarmonyPatch("Tick")]
    class Patch_ThingWithCompsTick_Prefix
    {
        public static bool Prefix(ThingWithComps __instance)
        {
            if(TimeControlBase.partialTick < 1.0 && __instance is Pawn) // force normal tick
            {
                return false;
            }
            return true;
        }

    }
}
