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
using Verse.AI;

namespace DTimeControl.Core_Patches.JobTracker_Patches
{
    [HarmonyPatch(typeof(Pawn_JobTracker))]
    [HarmonyPatch("JobTrackerTick")]
    class Patch_JobTrackerTick_Postfix
    {
        public static bool Prefix(Pawn_JobTracker __instance)
        {
            FieldInfo p = AccessTools.Field(typeof(Pawn_JobTracker), "pawn");
            Pawn pawn = p.GetValue(__instance) as Pawn;
            if (TickUtility.NoOverlapAdjustedIsHashIntervalTick(pawn, 30))
            {
                ThinkResult thinkResult = (ThinkResult)JobTrackerUtility.DetermineNextConstantThinkTreeJob.Invoke(__instance, null);
                if (thinkResult.IsValid)
                {
                    bool start = (bool)JobTrackerUtility.ShouldStartJobFromThinkTree.Invoke(__instance, new object[] { thinkResult });
                    if(start)
                    {
                        JobTrackerUtility.CheckLeaveJoinableLordBecauseJobIssued.Invoke(__instance, new object[] { thinkResult });
                        __instance.StartJob(thinkResult.Job, JobCondition.InterruptForced, thinkResult.SourceNode, false, false, pawn.thinker.ConstantThinkTree, thinkResult.Tag, false, false);
                    }
                    else if (thinkResult.Job != __instance.curJob && !__instance.jobQueue.Contains(thinkResult.Job))
                    {
                        JobMaker.ReturnToPool(thinkResult.Job);
                    }
                }
            }
            return true;
        }

    }
}
