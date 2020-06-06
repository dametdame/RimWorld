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
    [HarmonyPatch(typeof(Pawn_InventoryTracker))]
    [HarmonyPatch("InventoryTrackerTick")]
    class Patch_InventoryTrackerTick_Prefix
    {

        public static bool Prefix()
        {
            if (TimeControlBase.partialTick < 1.0)
                return false;
            return true;
        }

    }
}
