using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using RimWorld;
using Verse.Noise;

namespace DTimeControl
{
    class TimeControlSettings : ModSettings
    {

        public static float speedMultiplier = 1.25f;
        public static bool scalePawns = true;
        public static bool dontScale = false;
        public static bool showAdvanced = false;
        public static bool speedUnlocked = false;
        public static bool speedReallyUnlocked = false;
        public static bool slowWork = true;

        public static bool useMultithreading = true;

        public static int min = 90;
        public static int max = 300;

        public override void ExposeData()
        {
            Scribe_Values.Look<float>(ref speedMultiplier, "speedMultiplier", 1.25f);
            Scribe_Values.Look<bool>(ref scalePawns, "scalePawns", true);
            Scribe_Values.Look<bool>(ref dontScale, "dontScale", false);
            Scribe_Values.Look<bool>(ref showAdvanced, "showAdvanced", false);
            Scribe_Values.Look<bool>(ref speedUnlocked, "speedUnlocked", false);
            Scribe_Values.Look<bool>(ref speedReallyUnlocked, "speedReallyUnlocked", false);
            Scribe_Values.Look<bool>(ref slowWork, "slowWork", true);
            Scribe_Values.Look<bool>(ref useMultithreading, "useMultithreading", true);
            base.ExposeData();
        }

        public static void WriteAll() // called when settings window closes
        {
            if (!showAdvanced)
            {
                scalePawns = true;
                dontScale = false;
                speedUnlocked = false;
                speedReallyUnlocked = false;
            }
            if (dontScale)
            {
                scalePawns = false;
            }
            if (!speedUnlocked)
            {
                speedReallyUnlocked = false;
            }
            speedMultiplier = Mathf.Clamp(speedMultiplier, min / 100f, max / 100f);
            TimeControlBase.SetCycleLength();
        }

        public static void DrawSettings(Rect rect)
        {
            Listing_Standard ls = new Listing_Standard(GameFont.Small);
            ls.ColumnWidth = rect.width - 30f;
            ls.Begin(rect);
            ls.Gap();

            ls.CheckboxLabeled("Use multithreading (experimental)", ref useMultithreading, "Use at your own risk");
            ls.GapLine();

            
            if (speedReallyUnlocked && speedUnlocked)
            {
                min = 1;
                max = 1000;
            }
            else
            {
                min = 90;
                max = 300;
                speedMultiplier = Mathf.Clamp(speedMultiplier, min/100f, max/100f);
            }

            Rect timeMultRect = ls.GetRect(Text.LineHeight);
            Rect timeMultLabelRect = timeMultRect.LeftPartPixels(300);
            Rect timeMultSliderRect = timeMultRect.RightPartPixels(timeMultRect.width - 300);
            Widgets.Label(timeMultLabelRect, "Length of day multiplier");
            int multPercent = Mathf.RoundToInt(speedMultiplier * 100);
            multPercent = Mathf.RoundToInt(Widgets.HorizontalSlider(timeMultSliderRect, multPercent, min, max, middleAlignment: false, label: multPercent + "%", leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 1f));
            speedMultiplier = (float)multPercent / 100f;
            ls.Gap();

            ls.CheckboxLabeled("Keep work speed relatively the same per day", ref slowWork, "Disabling this will allow your colonists to get more work done in each day (if you make days longer), at the cost of making the game easier.");
            ls.Gap();

            ls.CheckboxLabeled("Show Advanced (Use at your own risk)", ref showAdvanced, "Use at your own risk.");
            ls.Gap();
            if (showAdvanced)
            {
                ls.CheckboxLabeled("Don't scale anything (not recommended)", ref dontScale, "Don't scale anything. This basically just makes this mod a custom game speed controller.");
                ls.Gap();
                if (!dontScale)
                {
                    ls.CheckboxLabeled("Scale pawns with length of day (recommended)", ref scalePawns,
                    "This will make sure your colonists eat, sleep, mine, craft, and so on at the normal rate per day of the base game. Disabling may cause weird (but potentially interesting?) things to happen. You've been warned.");
                }
                else
                {
                    ls.Label("Scale pawns with length of day (recommended)");
                }
                ls.Gap();
                ls.CheckboxLabeled("Unlock speed slider", ref speedUnlocked, "This is probably a bad idea.");
                ls.Gap();
                if (speedUnlocked)
                {
                    ls.CheckboxLabeled("Really? Are you sure? Extreme settings will cause... strange things to happen", ref speedReallyUnlocked, "You've been warned.");
                }
                
            }
            ls.Gap();
            ls.End();
        }

    }


}
