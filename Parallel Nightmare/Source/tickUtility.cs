using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using System.Runtime.Remoting.Messaging;

namespace DTimeControl
{
    [StaticConstructorOnStartup]
    public static class TickUtility
    {
        public static TickManager tickManager;

        public static TCTickList tickListNormal = null;
        public static TCTickList tickListRare = null;
        public static TCTickList tickListLong = null;

        public static FieldInfo tgi;
        public static FieldInfo las;

        public static Type FilthMonitor;
        public static MethodInfo fmt;

        public static int ticksGameInt
        {
            get
            {
                return tickManager.TicksGame;
            }
            set
            {
                tgi.SetValue(tickManager, value);
            }
        }

        public static int AdjustedTicksGame
        {
            get
            {
                return adjustedTicksGameInt;
            } 
        }

        public static int adjustedTicksGameInt = 0;


        public static bool AdjustedIsHashIntervalTick(this Thing t, int interval)
        {
            return t.AdjustedHashOffsetTicks() % interval == 0;
        }

        public static int AdjustedHashOffsetTicks(this Thing t)
        {
            return adjustedTicksGameInt + t.thingIDNumber.HashOffset();
        }

        public static bool NoOverlapAdjustedIsHashIntervalTick(this Thing t, int interval)
        {
            return t.AdjustedIsHashIntervalTick(interval) && !t.IsHashIntervalTick(interval);
        }

        public static bool NoOverlapTickMod(int interval)
        {
            return (adjustedTicksGameInt % interval == 0) && (ticksGameInt % interval != 0);
        }

        public static int lastAutoScreenshot
        {
            get
            {
                return (int)las.GetValue(tickManager);
            }
            set
            {
                las.SetValue(tickManager, value);
            }
        }

        public static void FilthMonitorTick()
        {
            fmt.Invoke(null, null);
        }

        static TickUtility()
        {
            FilthMonitor = AccessTools.TypeByName("RimWorld.FilthMonitor");
            fmt = AccessTools.Method(FilthMonitor, "FilthMonitorTick");
        }

        public static void GetManagerData(Game currentGame)
        {
            tickManager = currentGame.tickManager;

            FieldInfo tln = AccessTools.Field(typeof(TickManager), "tickListNormal");
            tickListNormal = tln.GetValue(tickManager) as TCTickList;
            FieldInfo tlr = AccessTools.Field(typeof(TickManager), "tickListRare");
            tickListRare = tlr.GetValue(tickManager) as TCTickList;
            FieldInfo tll = AccessTools.Field(typeof(TickManager), "tickListLong");
            tickListLong = tll.GetValue(tickManager) as TCTickList;

            tgi = AccessTools.Field(typeof(TickManager), "ticksGameInt");
            las = AccessTools.Field(typeof(TickManager), "lastAutoScreenshot");
        }

    }
}
