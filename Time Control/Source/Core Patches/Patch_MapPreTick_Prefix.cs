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

namespace DTimeControl
{
    [HarmonyPatch(typeof(Map))]
    [HarmonyPatch("MapPreTick")]
    class Patch_MapPreTick_Prefix
    {

		public static bool Prefix(Map __instance)
		{
			__instance.itemAvailability.Tick();
			__instance.listerHaulables.ListerHaulablesTick();
			try
			{
				__instance.autoBuildRoofAreaSetter.AutoBuildRoofAreaSetterTick_First();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString(), false);
			}
			__instance.roofCollapseBufferResolver.CollapseRoofsMarkedToCollapse();
			__instance.windManager.WindManagerTick();
			if (TimeControlBase.partialTick >= 1.0) {
				try
				{
					__instance.mapTemperature.MapTemperatureTick();
				}
				catch (Exception ex2)
				{
					Log.Error(ex2.ToString(), false);
				}
			}
			return false;
		}
    }
}
