using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using System.Reflection;
using UnityEngine;

namespace DSkillPoints.CorePatches
{
	[HarmonyPatch(typeof(ITab_Pawn_Character))]
	[HarmonyPatch("UpdateSize")]
	class Patch_UpdateSize_Post
    {
		private static void Postfix(ref ITab_Pawn_Character __instance)
		{
			FieldInfo size = AccessTools.Field(typeof(ITab_Pawn_Character), "size");
			Vector2 curSize = (Vector2)size.GetValue(__instance);
			curSize.x += Base.panelAddedWidth;
			size.SetValue(__instance, curSize);
		}
	}
}
