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
    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("HealthTick")]
    class Patch_HealthTick_Prefix
    {

        public static bool Prefix()
        {
            if (TimeControlBase.partialTick < 1.0 && TimeControlSettings.scalePawns || (!TimeControlSettings.scalePawns && TimeControlBase.cycleLength > 1 && Find.TickManager.TicksGame % TimeControlBase.cycleLength == 0))
                return false;
            return true;
        }

    }
}
