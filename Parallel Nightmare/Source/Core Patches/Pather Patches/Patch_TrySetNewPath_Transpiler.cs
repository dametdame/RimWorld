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

namespace DTimeControl.Core_Patches.Pather_Patches
{
	[HarmonyPatch(typeof(Pawn_PathFollower))]
	[HarmonyPatch("TrySetNewPath")]
	class Patch_TrySetNewPath_Transpiler
    {
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return GenericTickReplacer.ReplaceTicks(instructions, "TrySetNewPath");
		}

	}
}
