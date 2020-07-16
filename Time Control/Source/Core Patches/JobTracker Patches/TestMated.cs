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
    [HarmonyPatch(typeof(PawnUtility))]
    [HarmonyPatch("Mated")]
    class TestMated
    {
        public static bool Prefix(Pawn male, Pawn female)
        {
            Log.Message(male + " mated with " + female);
            return true;
        }
    }
}
