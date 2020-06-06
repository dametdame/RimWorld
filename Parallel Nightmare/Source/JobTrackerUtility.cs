using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace DTimeControl
{
    [StaticConstructorOnStartup]
    public static class JobTrackerUtility
    {

        public static MethodInfo DetermineNextConstantThinkTreeJob;
        public static MethodInfo ShouldStartJobFromThinkTree;
        public static MethodInfo CheckLeaveJoinableLordBecauseJobIssued;

        static JobTrackerUtility()
        {
            DetermineNextConstantThinkTreeJob = AccessTools.Method(typeof(Pawn_JobTracker), "DetermineNextConstantThinkTreeJob");
            ShouldStartJobFromThinkTree = AccessTools.Method(typeof(Pawn_JobTracker), "ShouldStartJobFromThinkTree");
            CheckLeaveJoinableLordBecauseJobIssued = AccessTools.Method(typeof(Pawn_JobTracker), "CheckLeaveJoinableLordBecauseJobIssued");
        }
    }
}
