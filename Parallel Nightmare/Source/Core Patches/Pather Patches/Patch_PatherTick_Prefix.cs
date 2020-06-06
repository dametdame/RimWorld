using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using RimWorld;
using HarmonyLib;
using Verse;
using UnityEngine;
using Verse.AI;
/*
namespace DTimeControl.Core_Patches.Pather_Patches
{
    [HarmonyPatch(typeof(Pawn_PathFollower))]
    [HarmonyPatch("PatherTick")]
    class Patch_PatherTick_Prefix
    {
        public static bool Prefix()
        {
            if (!TimeControlSettings.scalePawns && Find.TickManager.TicksGame % TimeControlBase.cycleLength != 0)
                return false;
            return true;
        }

    }
}
*/