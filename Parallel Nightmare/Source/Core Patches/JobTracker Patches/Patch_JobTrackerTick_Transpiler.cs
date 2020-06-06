using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine.UI;
using Verse.AI;

namespace DTimeControl.Core_Patches.JobTracker_Patches
{
	[HarmonyPatch(typeof(Pawn_JobTracker))]
	[HarmonyPatch("JobTrackerTick")]
	class Patch_JobTrackerTick_Transpiler
    {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return GenericTickReplacer.ReplaceTicks(instructions, "JobTrackerTick");
		}

	}
}
