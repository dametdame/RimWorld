using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RimWorld;
using HarmonyLib;
using Verse;
using UnityEngine;

namespace DTimeControl.Core_Patches.Pawn_Timer_Adjustments
{
    [HarmonyPatch(typeof(Pawn_NeedsTracker))]
    [HarmonyPatch("NeedsTrackerTick")]
    class Patch_NeedsTrackerTick_Prefix
    {

        public static bool Prefix()
        {
            if (TimeControlBase.partialTick < 1.0 && TimeControlSettings.scalePawns)
            {
                return false;
            }
            return true;
        }

    }
}
