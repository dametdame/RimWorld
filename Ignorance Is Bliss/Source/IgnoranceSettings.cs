using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;
using DIgnoranceIsBliss.Core_Patches;

namespace DIgnoranceIsBliss
{
    class IgnoranceSettings : ModSettings
    {

        public static bool useHighestResearched = false;
        public static bool usePercentResearched = true;
        public static float percentResearchNeeded = 0.25f;
        public static bool useActualTechLevel = false;
        public static bool useFixedTechRange = false;
        public static IntRange fixedRange = new IntRange(1, 7);

        public static int numTechsBehind = -1;
        public static int numTechsAhead = 1;

        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref useHighestResearched, "useHighestResearched", false);
            Scribe_Values.Look<bool>(ref useHighestResearched, "usePercentResearched", true);
            Scribe_Values.Look<float>(ref percentResearchNeeded, "percentResearchNeeded", 0.25f);
            Scribe_Values.Look<bool>(ref useActualTechLevel, "useActualTechLevel", false);
            Scribe_Values.Look<bool>(ref useFixedTechRange, "useFixedTechRange", false);
            Scribe_Values.Look<IntRange>(ref fixedRange, "fixedRange", new IntRange(1, 7));
            Scribe_Values.Look<int>(ref numTechsBehind, "numTechsBehind", -1);
            Scribe_Values.Look<int>(ref numTechsAhead, "numTechsAhead", 1);
            base.ExposeData();
        }

        public static void WriteAll() // called when settings window closes
        {
            if (useHighestResearched)
            {
                usePercentResearched = false;
                useActualTechLevel = false;
                useFixedTechRange = false;
            }
            else if (usePercentResearched)
            {
                useHighestResearched = false;
                useActualTechLevel = false;
                useFixedTechRange = false;
            }
            else if (useActualTechLevel)
            {
                useHighestResearched = false;
                usePercentResearched = false;
                useFixedTechRange = false;
            }
            else if (useFixedTechRange)
            {
                usePercentResearched = false;
                useActualTechLevel = false;
                useHighestResearched = false;
            }
            if (Current.Game != null)
            {
                IgnoranceBase.playerTechLevel = IgnoranceBase.GetPlayerTech();
                IgnoranceBase.WarnIfNoFactions();
            }
        }

        public static void DrawSettings(Rect rect)
        {
            Listing_Standard ls = new Listing_Standard(GameFont.Small);

            ls.ColumnWidth = rect.width - 30f;
            ls.Begin(rect);
            ls.Gap();
            bool a;
            bool b;
            bool c;
            bool d;

            ls.Gap();
            string s = "Your calculated tech level: ";
            if (useFixedTechRange)
                s += "N/A (fixed tech range)";
            else if (Current.Game != null)
                s += Enum.GetName(typeof(TechLevel), IgnoranceBase.playerTechLevel);
            else
                s += "Not in game";
            Rect standard = ls.Label(s);
            float standardHeight = standard.height;
            if (Current.Game != null) {
                
                if (!useFixedTechRange)
                {
                    TechLevel playerTech = IgnoranceBase.playerTechLevel;
                    ls.Label("Number of eligible, hostile factions " + 
                        "(below/equivalent to/above) your tech: " + IgnoranceBase.NumFactionsBelow(playerTech) + "/" + IgnoranceBase.NumFactionsEqual(playerTech) + "/" + IgnoranceBase.NumFactionsAbove(playerTech));
                    //Rect numFactionsRect = ls.GetRect(standardHeight);
                    //float oneThirdWidth = ls.ColumnWidth / 3f;
                    //Rect belowRect = numFactionsRect.LeftPartPixels(oneThirdWidth);
                    //Rect aboveRect = numFactionsRect.RightPartPixels(oneThirdWidth);
                    //Rect equalRect = new Rect(numFactionsRect.x + oneThirdWidth, numFactionsRect.y, oneThirdWidth, numFactionsRect.height);
                    //string below = "Below: " + IgnoranceBase.NumFactionsBelow(playerTech);
                    //string above = "Above: " + IgnoranceBase.NumFactionsAbove(playerTech);
                    //string equal = "Equivalent: " + IgnoranceBase.NumFactionsEqual(playerTech);
                    //Widgets.Label(belowRect, below);
                    //Widgets.Label(aboveRect, above);
                    //Widgets.Label(equalRect, equal);
                }
                else
                {
                    ls.Label("Number of eligible, hostile factions in range: " + IgnoranceBase.NumFactionsInRange());
                }
            }
            ls.Gap();
            ls.GapLine();

            ls.Label("Method by which this mod will calculate your tech level for raids and incidents:");
            d = ls.RadioButton_NewTemp("Fixed range", useFixedTechRange, tooltip: "Will not dynamically update with the game state");
            if (useFixedTechRange)
            {
                Rect fixedRangeRect = ls.GetRect(standardHeight);
                Rect fixedRangeLabelRect = fixedRangeRect.LeftPartPixels(450);
                //Rect fixedRangedSliderRect = fixedRangeRect.RightPartPixels(fixedRangeRect.width - 450);
                //Widgets.Label(fixedRangeLabelRect, "Range:");
                Widgets.IntRange(fixedRangeLabelRect, 999, ref fixedRange, 1, 7, Enum.GetName(typeof(TechLevel), fixedRange.min) + " - " + Enum.GetName(typeof(TechLevel), fixedRange.max));
            }
            else
            {
                ls.Gap();
            }
            ls.Gap();
            a = ls.RadioButton_NewTemp("Highest tech researched", useHighestResearched, tooltip: "If you have even one tech in a tech level researched, you will be considered that tech for the purpose of raids.");
            ls.Gap();
            b = ls.RadioButton_NewTemp("Tech completion of a certain percent", usePercentResearched, tooltip: "Once you research a certain % of a tech level's available technologies, you will be considered that tech level for the purpose of raids.");
            if (usePercentResearched)
            {
                percentResearchNeeded = Mathf.Clamp(percentResearchNeeded, 0.05f, 1f);
                Rect percentResearchRect = ls.GetRect(standardHeight);
                Rect percentResearchLabelRect = percentResearchRect.LeftPartPixels(450);
               // Rect percentResearchSliderRect = percentResearchRect.RightPartPixels(percentResearchRect.width - 450);
               // Widgets.Label(percentResearchLabelRect, "Percent required:");
                percentResearchNeeded = Widgets.HorizontalSlider(percentResearchLabelRect, percentResearchNeeded, 0.05f, 1f, middleAlignment: false, label: Mathf.RoundToInt(percentResearchNeeded*100) + "%", leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 0.05f);
            }
            else
            {
                ls.Gap();
            }
            ls.Gap();

            c = ls.RadioButton_NewTemp("Actual colonist tech level", useActualTechLevel, tooltip: "Not recommended unless you have a mod to increase your tech level somehow.");

            if (!useFixedTechRange)
            {
                ls.GapLine();

                ls.Label("Please note that there are 7 tech levels in the game by default.");
                ls.Label("Also keep in mind that there are NO medieval factions in the vanilla game.");
                ls.Label("(This means tribal starts will only fight other tribes at the start with default settings)");
                ls.Gap();
                ls.Label("Maximum difference between your calculated tech level and enemy faction's (-1 is any):");
                ls.Gap();
                Rect techsBehindRect = ls.GetRect(standardHeight);
                Rect techsBehindLabelRect = techsBehindRect.LeftPartPixels(450);
                //Rect techsBehindSliderRect = techsBehindRect.RightPartPixels(techsBehindRect.width - 450);
                //Widgets.Label(techsBehindLabelRect, "Max # tech levels behind to encounter (-1 is any)");
                numTechsBehind = Mathf.RoundToInt(Widgets.HorizontalSlider(techsBehindLabelRect, numTechsBehind, -1, 7f, middleAlignment: false, label: OffsetString(numTechsBehind) + " behind", leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 1));

                ls.Gap();
                Rect techsAheadRect = ls.GetRect(standardHeight);
                Rect techsAheadLabelRect = techsAheadRect.LeftPartPixels(450);
                //Rect techsAheadSliderRect = techsAheadRect.RightPartPixels(techsAheadRect.width - 450);
                //Widgets.Label(techsAheadLabelRect, "Max # tech levels ahead to encounter (-1 is any)");
                numTechsAhead = Mathf.RoundToInt(Widgets.HorizontalSlider(techsAheadLabelRect, numTechsAhead, -1, 7f, middleAlignment: false, label: OffsetString(numTechsAhead) + " ahead", leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 1));
            }
            if (a && a != useHighestResearched)
            {
                useHighestResearched = true;
                usePercentResearched = false;
                useActualTechLevel = false;
                useFixedTechRange = false;
                if (Current.Game != null)
                    IgnoranceBase.playerTechLevel = IgnoranceBase.GetPlayerTech();
            }
            else if (b && b != usePercentResearched)
            {
                useHighestResearched = false;
                usePercentResearched = true;
                useActualTechLevel = false;
                useFixedTechRange = false;
                if (Current.Game != null)
                    IgnoranceBase.playerTechLevel = IgnoranceBase.GetPlayerTech();
            }
            else if (c && c != useActualTechLevel)
            {
                useHighestResearched = false;
                usePercentResearched = false;
                useActualTechLevel = true;
                useFixedTechRange = false;
                if (Current.Game != null)
                    IgnoranceBase.playerTechLevel = IgnoranceBase.GetPlayerTech();
            }
            else if (d && d != useFixedTechRange)
            {
                useHighestResearched = false;
                usePercentResearched = false;
                useActualTechLevel = false;
                useFixedTechRange = true;
                if (Current.Game != null)
                    IgnoranceBase.playerTechLevel = IgnoranceBase.GetPlayerTech();
            }

            ls.Gap();
            ls.End();
        }

        public static string OffsetString(int num)
        {
            if (num < 0)
                return "Any number";
            else return num.ToString();
        }
    }
}
