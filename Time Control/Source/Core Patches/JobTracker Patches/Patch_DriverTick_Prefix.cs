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


namespace DTimeControl.Core_Patches.JobTracker_Patches
{
    [HarmonyPatch(typeof(JobDriver))]
    [HarmonyPatch("DriverTick")]
    class Patch_DriverTick_Prefix
    {
        public static bool Prefix(JobDriver __instance)
        {
            if (TimeControlBase.partialTick < 1.0 
                && (
                    (TimeControlSettings.scalePawns && TimeControlSettings.slowWork
                        && !(__instance is JobDriver_TendPatient
                                //|| (__instance is JobDriver_Wait || __instance is JobDriver_WaitDowned || __instance is JobDriver_WaitMaintainPosture)
                                || (__instance is JobDriver_Lovin || __instance is JobDriver_Mate)
                                // A RimWorld of Magic
                                || __instance.GetType().Name == "TMJobDriver_CastAbilityVerb" 
                                || __instance.GetType().Name == "TMJobDriver_CastAbilitySelf" 
                                || __instance.GetType().Name == "JobDriver_GotoAndCast") 
                        )
                    || (__instance is JobDriver_ChatWithPrisoner || __instance is JobDriver_Tame)           
                    ))
                return false;
            return true;
        }
    }
}
