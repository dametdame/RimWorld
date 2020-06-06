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
    [HarmonyPatch(typeof(Pawn_RelationsTracker))]
    [HarmonyPatch("Tick_CheckStartMarriageCeremony")]
    class Patch_Tick_CheckStartMarriageCeremony_Prefix
    {
        public static bool Prefix(ThingWithComps __instance)
        {
            if (TimeControlBase.partialTick < 1.0)
            {
                return false;
            }
            return true;
        }

    }
}
