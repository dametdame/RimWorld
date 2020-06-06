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
    [HarmonyPatch(typeof(TickManager))]
    [HarmonyPatch("DoSingleTick")]
    class Patch_DoSingleTick_Prefix
    {
        public static bool Prefix(TickManager __instance) 
        {
            TimeControlBase.TickManagerTick(__instance);
            return false;
        }

    }
}
