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

namespace DTimeControl.Core_Patches.Pawn_Timer_Adjustments
{
    [HarmonyPatch(typeof(Pawn_MindState))]
    [HarmonyPatch("MindStateTick")]
    class Patch_MindStateTick_Postfix
    {
        public static void Postfix(Pawn_MindState __instance)
        {
			if (__instance.pawn.NoOverlapAdjustedIsHashIntervalTick(100))
			{
				if (__instance.pawn.Spawned)
				{
					int regionsToScan = __instance.anyCloseHostilesRecently ? 24 : 18;
					__instance.anyCloseHostilesRecently = PawnUtility.EnemiesAreNearby(__instance.pawn, regionsToScan, true);
				}
				else
				{
					__instance.anyCloseHostilesRecently = false;
				}
			}
			if (TickUtility.NoOverlapTickMod(123) && __instance.pawn.Spawned && __instance.pawn.RaceProps.IsFlesh && __instance.pawn.needs.mood != null)
			{
				TerrainDef terrain = __instance.pawn.Position.GetTerrain(__instance.pawn.Map);
				if (terrain.traversedThought != null)
				{
					__instance.pawn.needs.mood.thoughts.memories.TryGainMemoryFast(terrain.traversedThought);
				}
				WeatherDef curWeatherLerped = __instance.pawn.Map.weatherManager.CurWeatherLerped;
				if (curWeatherLerped.exposedThought != null && !__instance.pawn.Position.Roofed(__instance.pawn.Map))
				{
					__instance.pawn.needs.mood.thoughts.memories.TryGainMemoryFast(curWeatherLerped.exposedThought);
				}
			}
		}

    }
}
