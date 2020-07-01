using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;

namespace DHotMeals
{
    class HotMealsSettings : ModSettings
    {

        
        public static bool positiveMoodEnabled = true;
        public static int positiveMoodBuff = 3;
        public static bool negativeMoodEnabled = true;
        public static int negativeMoodDebuff = -3;
        public static float heatSpeedMult = 1;
        public static bool thawIt = false;
        public static bool useCookingAppliances = true;
        public static bool multipleHeat = false;
        public static float searchRadius = 48f;

        public override void ExposeData()
        {
            
            Scribe_Values.Look(ref positiveMoodEnabled, "positiveMoodEnabled", true);
            Scribe_Values.Look(ref positiveMoodBuff, "positiveMoodBuff", 3);
            Scribe_Values.Look(ref negativeMoodEnabled, "negativeMoodEnabled", true);
            Scribe_Values.Look(ref negativeMoodDebuff, "negativeMoodDebuff", -3);
            Scribe_Values.Look(ref heatSpeedMult, "heatSpeedMult", 1);
            Scribe_Values.Look(ref thawIt, "thawIt", false);
            Scribe_Values.Look(ref useCookingAppliances, "useCookingAppliances", true);
            Scribe_Values.Look(ref multipleHeat, "multipleHeat", false);
            Scribe_Values.Look(ref searchRadius, "searchRadius", 48f);
            base.ExposeData();
        }

        public static void WriteAll() // called when settings window closes
        {
        }

        public static void DrawSettings(Rect rect)
        {
            Listing_Standard ls = new Listing_Standard(GameFont.Small);

            ls.ColumnWidth = rect.width * 2.0f / 3.0f;
            ls.Begin(rect);
            ls.Gap();

            ls.CheckboxLabeled("Multiple pawns can heat at same building", ref multipleHeat, "Pawns won't reserve buildings when they heat food, so multiple pawns will be able to use the same appliance to reheat meals. Default off.");
            ls.Gap();

            Rect searchRect = ls.GetRect(Text.LineHeight);
            Rect searchLabelRect = searchRect.LeftPartPixels(300);
            Rect searchSliderRect = searchRect.RightPartPixels(searchRect.width - 300);
            Widgets.Label(searchLabelRect, "Heater search radius (default 48)");
            searchRadius = Widgets.HorizontalSlider(searchSliderRect, searchRadius, 1f, 100f, middleAlignment: false, label: searchRadius.ToString("f0"), leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 1f);
            ls.Gap();

            Rect heatRect = ls.GetRect(Text.LineHeight);
            Rect heatLabelRect = heatRect.LeftPartPixels(300);
            Rect heatSliderRect = heatRect.RightPartPixels(heatRect.width - 300);
            Widgets.Label(heatLabelRect, "Heating speed (at stove etc.) (default 1.0)");
            heatSpeedMult = Widgets.HorizontalSlider(heatSliderRect, heatSpeedMult, 0.1f, 5.0f, middleAlignment: false, label: heatSpeedMult.ToString("f1"), leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 0.1f);
            ls.Gap();
            ls.Gap();
            ls.CheckboxLabeled("Enable positive moodlets from good food temperature", ref positiveMoodEnabled);
            ls.Gap();
            if (positiveMoodEnabled)
            {
                Rect posRect = ls.GetRect(Text.LineHeight);
                Rect posLabelRect = posRect.LeftPartPixels(300);
                Rect posSliderRect = posRect.RightPartPixels(posRect.width - 300);
                Widgets.Label(posLabelRect, "Mood bonus (default 3)");
                positiveMoodBuff = Mathf.RoundToInt(Widgets.HorizontalSlider(posSliderRect, positiveMoodBuff, 1, 20, middleAlignment: false, label: positiveMoodBuff.ToString(), leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 1));
            }
            ls.Gap();
            ls.CheckboxLabeled("Enable negative moodlets from bad food temperature", ref negativeMoodEnabled);
            if (negativeMoodEnabled)
            {
                Rect negRect = ls.GetRect(Text.LineHeight);
                Rect negLabelRect = negRect.LeftPartPixels(300);
                Rect negSliderRect = negRect.RightPartPixels(negRect.width - 300);
                Widgets.Label(negLabelRect, "Mood penalty per level (default -3)");
                negativeMoodDebuff = Mathf.RoundToInt(Widgets.HorizontalSlider(negSliderRect, negativeMoodDebuff, -15, 0, middleAlignment: false, label: negativeMoodDebuff.ToString(), leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 1));
            }
            ls.Gap();
            ls.Gap();
            ls.CheckboxLabeled("Frozen things will be thawed by pawns before consuming", ref thawIt);

            ls.Gap();
            ls.CheckboxLabeled("Pawns will use unreserved cooking appliances (stoves etc.) to heat", ref useCookingAppliances);

            ls.End();
        }

    }
}
