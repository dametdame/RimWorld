using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RimWorld;
using HarmonyLib;
using Verse;
using UnityEngine;
using System.Reflection;


namespace DTimeControl.Core_Patches
{
    [HarmonyPatch(typeof(ResourceCounter))]
    [HarmonyPatch("ResourceCounterTick")]
    class Patch_ResourceCounterTick_Postfix
    {
        public static void Postfix(ResourceCounter __instance)
        {
            if (TickUtility.NoOverlapTickMod(204))
            {
                __instance.UpdateResourceCounts();
            }
        }

    }
}
