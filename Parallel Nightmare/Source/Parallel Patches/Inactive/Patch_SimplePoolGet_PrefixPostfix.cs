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
/*
namespace DTimeControl.Parallel_Nightmare
{
    class Patch_SimplePoolGet_PrefixPostfix
    {

        public static bool Prefix(ref object __instance)
        {
            Log.Message("Simplepool lock");
            LockUtility.LockObject(ref __instance);
            return true;
        }

        public static void Postfix(ref object __instance)
        {
            Log.Message("Simplepool unlock");
            LockUtility.UnLockObject(ref __instance);
        }
    }
}
*/