using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace DTimeControl
{
    [StaticConstructorOnStartup]
    public static class TimeControlBase
    {

        public static double partialTick = 0;

		public static int cycleLength = 1;

		static TimeControlBase()
		{
		}

		public static void SetCycleLength()
		{
			if (Current.Game == null)
				return;
			if (1.0 / TimeControlSettings.speedMultiplier > 1)
                TimeControlBase.cycleLength = Mathf.RoundToInt(1.0f / TimeControlSettings.speedMultiplier);
            else
                TimeControlBase.cycleLength = 1;
            TickUtility.tickListNormal.cycleStep = 0;
            TickUtility.tickListRare.cycleStep = 0;
            TickUtility.tickListLong.cycleStep = 0;
		}

		public static void TickManagerTick(TickManager tm, bool firstRun = true)
        {
			float mult = TimeControlSettings.speedMultiplier;
			if (firstRun)
			{
				partialTick += 1.0 / mult;
			}

			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				maps[i].MapPreTick();
			}
			if (partialTick >= 1.0)
			{
				if (!DebugSettings.fastEcology)
				{
					TickUtility.ticksGameInt++;
				}
				else
				{
					TickUtility.ticksGameInt += 2000;
				}
			}
			if (firstRun || TimeControlSettings.dontScale)
			{
				if (!DebugSettings.fastEcology)
				{
					TickUtility.adjustedTicksGameInt++;
				}
				else
				{
					TickUtility.adjustedTicksGameInt += 2000;
				}
			}
			Shader.SetGlobalFloat(ShaderPropertyIDs.GameSeconds, tm.TicksGame.TicksToSeconds() * mult);
			TickUtility.tickListNormal.DoTick(partialTick, firstRun);
			TickUtility.tickListRare.DoTick(partialTick, firstRun);
			TickUtility.tickListLong.DoTick(partialTick, firstRun);
			if (partialTick >= 1.0)
			{
				try
				{
					
					Find.DateNotifier.DateNotifierTick();
				}
				catch (Exception ex)
				{
					Log.Error(ex.ToString(), false);
				}
				try
				{
					Find.Scenario.TickScenario();
				}
				catch (Exception ex2)
				{
					Log.Error(ex2.ToString(), false);
				}
				try
				{
					Find.World.WorldTick();
				}
				catch (Exception ex3)
				{
					Log.Error(ex3.ToString(), false);
				}
				try
				{
					Find.StoryWatcher.StoryWatcherTick();
				}
				catch (Exception ex4)
				{
					Log.Error(ex4.ToString(), false);
				}
				try
				{
					Find.GameEnder.GameEndTick();
				}
				catch (Exception ex5)
				{
					Log.Error(ex5.ToString(), false);
				}
				try
				{
					Find.Storyteller.StorytellerTick();
				}
				catch (Exception ex6)
				{
					Log.Error(ex6.ToString(), false);
				}
				try
				{
					Find.TaleManager.TaleManagerTick();
				}
				catch (Exception ex7)
				{
					Log.Error(ex7.ToString(), false);
				}
				try
				{
					Find.QuestManager.QuestManagerTick();
				}
				catch (Exception ex8)
				{
					Log.Error(ex8.ToString(), false);
				}
				try
				{
					Find.World.WorldPostTick();
				}
				catch (Exception ex9)
				{
					Log.Error(ex9.ToString(), false);
				}
				for (int j = 0; j < maps.Count; j++)
				{
					maps[j].MapPostTick();
				}
				try
				{
					Find.History.HistoryTick();
				}
				catch (Exception ex10)
				{
					Log.Error(ex10.ToString(), false);
				}
				GameComponentUtility.GameComponentTick();
				try
				{
					Find.LetterStack.LetterStackTick();
				}
				catch (Exception ex11)
				{
					Log.Error(ex11.ToString(), false);
				}
				try
				{
					Find.Autosaver.AutosaverTick();
				}
				catch (Exception ex12)
				{
					Log.Error(ex12.ToString(), false);
				}
				if (DebugViewSettings.logHourlyScreenshot && Find.TickManager.TicksGame >= TickUtility.lastAutoScreenshot + 2500)
				{
					ScreenshotTaker.QueueSilentScreenshot();
					TickUtility.lastAutoScreenshot = Find.TickManager.TicksGame / 2500 * 2500;
				}
				try
				{
					TickUtility.FilthMonitorTick();
				}
				catch (Exception ex13)
				{
					Log.Error(ex13.ToString(), false);
				}
				partialTick -= 1.0;
			}
			else
			{
				for (int j = 0; j < maps.Count; j++)
				{
					maps[j].resourceCounter.ResourceCounterTick();
				}
			}
			UnityEngine.Debug.developerConsoleVisible = false;
			if (partialTick >= 1.0)
				TickManagerTick(tm, false);
		}


		public static int HourMinuteReadout(long absTicks, float longitude)
		{
			Log.Message("moo");
			return GenDate.HourInteger(absTicks, longitude);
		}
    }
}
