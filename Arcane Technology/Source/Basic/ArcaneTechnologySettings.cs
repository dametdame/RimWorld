using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;
using Verse.Noise;

namespace DArcaneTechnology
{
    class ArcaneTechnologySettings : ModSettings
    {
        public static bool useHighestResearched = false;
        public static bool usePercentResearched = true;
        public static float percentResearchNeeded = 0.25f;
        public static bool useActualTechLevel = false;
        public static TechLevel minToRestrict = TechLevel.Spacer;
        public static bool restrictOnTechLevel = false;
        public static bool evenResearched = false;
        public static bool exemptClothing = true;

        public override void ExposeData()
        {
            
            Scribe_Values.Look(ref useHighestResearched, "useHighestResearched", false);
            Scribe_Values.Look(ref usePercentResearched, "usePercentResearched", true);
            Scribe_Values.Look(ref percentResearchNeeded, "percentResearchNeeded", 0.25f);
            Scribe_Values.Look(ref useActualTechLevel, "useActualTechLevel", false);
            Scribe_Values.Look(ref minToRestrict, "minToRestrict", TechLevel.Spacer);
            Scribe_Values.Look(ref restrictOnTechLevel, "restrictOnTechLevel", false);
            Scribe_Values.Look(ref evenResearched, "evenResearched", false);
            Scribe_Values.Look(ref exemptClothing, "exemptClothing", true);
            base.ExposeData();
        }

        public static void WriteAll() // called when settings window closes
        {
            if (useHighestResearched)
            {
                usePercentResearched = false;
                useActualTechLevel = false;
            }
            else if (usePercentResearched)
            {
                useHighestResearched = false;
                useActualTechLevel = false;
            }
            else if (useActualTechLevel)
            {
                useHighestResearched = false;
                usePercentResearched = false;
            }
            if (Current.Game != null)
            {
                Base.playerTechLevel = Base.GetPlayerTech();
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

            string s = "Your calculated tech level: ";
            if (!restrictOnTechLevel)
                s += "N/A (fixed tech level)";
            else if (Current.Game != null)
                s += Enum.GetName(typeof(TechLevel), Base.playerTechLevel);
            else
                s += "Not in game";
            Rect standard = ls.Label(s);
            float standardHeight = standard.height;
            ls.GapLine();

            ls.CheckboxLabeled("Try to exempt clothing research options", ref exemptClothing, "Refers to a list of clothing research projects and exempts their products from restriction.");
            ls.GapLine();

            ls.CheckboxLabeled("Restrict items above the colony's current tech level", ref restrictOnTechLevel, "Instead of manually setting the tech level above which to restrict tech, you can automatically use the colony's tech level.");
            ls.Gap();
            ls.GapLine();
            if (restrictOnTechLevel)
            {
                ls.Label("Method by which this mod will calculate your tech level:");
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
                    percentResearchNeeded = Widgets.HorizontalSlider(percentResearchLabelRect, percentResearchNeeded, 0.05f, 1f, middleAlignment: false, label: Mathf.RoundToInt(percentResearchNeeded * 100) + "%", leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 0.05f);
                }
                else
                {
                    ls.Gap();
                }
                ls.Gap();

                c = ls.RadioButton_NewTemp("Actual colonist tech level", useActualTechLevel, tooltip: "Not recommended unless you have a mod to increase your tech level somehow.");

                if (a && a != useHighestResearched)
                {
                    useHighestResearched = true;
                    usePercentResearched = false;
                    useActualTechLevel = false;
                    if (Current.Game != null)
                        Base.playerTechLevel = Base.GetPlayerTech();
                }
                else if (b && b != usePercentResearched)
                {
                    useHighestResearched = false;
                    usePercentResearched = true;
                    useActualTechLevel = false;
                    if (Current.Game != null)
                        Base.playerTechLevel = Base.GetPlayerTech();
                }
                else if (c && c != useActualTechLevel)
                {
                    useHighestResearched = false;
                    usePercentResearched = false;
                    useActualTechLevel = true;
                    if (Current.Game != null)
                        Base.playerTechLevel = Base.GetPlayerTech();
                }
            }
            else
            {
                Rect techLevelRect = ls.GetRect(Text.LineHeight);
                Rect techLabelRect = techLevelRect.LeftPartPixels(300);
                Rect techSliderRect = techLevelRect.RightPartPixels(techLevelRect.width - 300);
                Widgets.Label(techLabelRect, "Minimum tech level to restrict items");
                minToRestrict = (TechLevel)Mathf.RoundToInt(Widgets.HorizontalSlider(techSliderRect, (float)minToRestrict, (float)TechLevel.Animal, (float)TechLevel.Archotech, middleAlignment: false, label: Enum.GetName(typeof(TechLevel), (int)minToRestrict), leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 1f));
            }
            ls.GapLine();
            ls.CheckboxLabeled("Restrict even if the item is researched", ref evenResearched, "Warning: without a mod to let your colony advance in tech, you will NEVER be able to use certain items.");
            
            ls.Gap();

            ls.Gap();
            ls.End();
        }

    }


}
