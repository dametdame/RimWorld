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
    [HarmonyPatch(typeof(GenSpawn))]
    [HarmonyPatch("Spawn")]
    [HarmonyPatch(new Type[] {typeof(Thing), typeof(IntVec3), typeof(Map), typeof(Rot4), typeof(WipeMode), typeof(bool) })]
    class Patch_GenSpawnSpawn
        {
        public static bool Prefix(object newThing)
        {
            LockUtility.LockObject(ref newThing);
            return true;
        }

        public static void Postfix(object newThing)
        {
            LockUtility.UnLockObject(ref newThing);
        }
    }
}
*/