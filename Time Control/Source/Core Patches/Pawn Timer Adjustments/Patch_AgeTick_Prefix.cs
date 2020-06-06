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
    [HarmonyPatch(typeof(Pawn_AgeTracker))]
    [HarmonyPatch("AgeTick")]
    class Patch_AgeTick_Prefix
    {

        public static bool Prefix()
        {
            if (TimeControlBase.partialTick < 1.0 && TimeControlSettings.scalePawns)
                return false; 
            return true;
        }

    }
}
