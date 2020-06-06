using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;
using UnityEngine;
using Verse.Sound;
/*
namespace DTimeControl.Parallel_Nightmare
{
    [HarmonyPatch(typeof(SustainerManager))]
    [HarmonyPatch("UpdateAllSustainerScopes")]
    class Patch_UpdateAllSustainerScopes
    {
        public static bool Prefix(object __instance)
        {
            //Log.Message("Sustainer updateallinscope lock");
            LockUtility.LockObject(ref __instance);
            return true;
        }

        public static void Postfix(object __instance)
        {
            //Log.Message("Sustainer updateallinscope unlock");
            LockUtility.UnLockObject(ref __instance);
        }
    }
}
*/