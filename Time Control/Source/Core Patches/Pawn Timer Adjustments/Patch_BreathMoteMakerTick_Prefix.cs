using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RimWorld;
using HarmonyLib;
using Verse;
using UnityEngine;

namespace DTimeControl
{
    [HarmonyPatch(typeof(PawnBreathMoteMaker))]
    [HarmonyPatch("BreathMoteMakerTick")]
    class Patch_BreathMoteMakerTick_Prefix
    {
        public static bool Prefix()
        {
            if (TimeControlBase.partialTick < 1.0)
                return false;
            return true;
        }

    }
}
