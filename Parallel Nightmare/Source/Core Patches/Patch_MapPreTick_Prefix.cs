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
			if (TimeControlSettings.useMultithreading)
			{
				Task avail = new Task(__instance.itemAvailability.Tick);
				Task haul = new Task(__instance.listerHaulables.ListerHaulablesTick);
				Task roof = new Task(__instance.autoBuildRoofAreaSetter.AutoBuildRoofAreaSetterTick_First);
				Task collapse = new Task(__instance.roofCollapseBufferResolver.CollapseRoofsMarkedToCollapse);
				Task wind = new Task(__instance.windManager.WindManagerTick);
				avail.Start();
				haul.Start();
				roof.Start();
				collapse.Start();
				wind.Start();
				if (TimeControlBase.partialTick >= 1.0)
				{
					Task temp = new Task(__instance.mapTemperature.MapTemperatureTick);
					temp.Start();
				}
				try
				{
					Task.WaitAll();
				}
				catch (AggregateException ae)
				{
					Log.Error(ae.ToString(), false);
				}
			}
			else
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
				if (TimeControlBase.partialTick >= 1.0)
				{
					try
					{
						__instance.mapTemperature.MapTemperatureTick();
					}
					catch (Exception ex2)
					{
						Log.Error(ex2.ToString(), false);
					}
					
				}
			}
			return false;
		}
    }
}
