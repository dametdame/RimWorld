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
    [HarmonyPatch(typeof(TimeSlower))]
    [HarmonyPatch("SignalForceNormalSpeed")]
    class Patch_SignalForceNormalSpeed_Prefix
    {
        public static bool Prefix(TimeSlower __instance)
        {
            FieldInfo fnsu = AccessTools.Field(typeof(TimeSlower), "forceNormalSpeedUntil");
            int forceUntil = (int)(Find.TickManager.TicksGame + 800f/TimeControlSettings.speedMultiplier);
            fnsu.SetValue(__instance, forceUntil);
            return false;
        }

    }
}
