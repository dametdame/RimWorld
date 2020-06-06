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
    [HarmonyPatch("SignalForceNormalSpeedShort")]
    class Patch_SignalForceNormalSpeedShort_Prefix
    {
        public static bool Prefix(TimeSlower __instance)
        {
            FieldInfo fnsu = AccessTools.Field(typeof(TimeSlower), "forceNormalSpeedUntil");
            int currentForce = (int)fnsu.GetValue(__instance);
            int forceUntil = (int)Mathf.Max(currentForce, Find.TickManager.TicksGame + 240f / TimeControlSettings.speedMultiplier);
            fnsu.SetValue(__instance, forceUntil);
            return false;
        }

    }
}