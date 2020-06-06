using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;


namespace DActiveCircuits
{
    class ActiveCircuitsSettings : ModSettings
    {

        private const int rectWidth = 7;

        public static Color salmon = new UnityEngine.Color(250f / 255f, 127f / 255f, 114f / 255f);

        public static Color justConduits = Color.white;
        public static Color nothingConnected = Color.magenta;
        public static Color noPower = Color.red;
        public static Color drainingPower = salmon;
        public static Color hasPower = Color.yellow;
        public static Color surplusPower = Color.green;

        public static bool displayBadOnMain = false;

        public override void ExposeData()
        {
            Scribe_Values.Look<Color>(ref justConduits, "justConduits", Color.white, false);
            Scribe_Values.Look<Color>(ref nothingConnected, "nothingConnected", Color.magenta, false);
            Scribe_Values.Look<Color>(ref noPower, "noPower", Color.red, false);
            Scribe_Values.Look<Color>(ref drainingPower, "drainingPower", salmon, false);
            Scribe_Values.Look<Color>(ref hasPower, "hasPower", Color.yellow, false);
            Scribe_Values.Look<Color>(ref surplusPower, "surplusPower", Color.green, false);
            Scribe_Values.Look<bool>(ref displayBadOnMain, "displayBadOnMain", false, false);
            base.ExposeData();
        }

        public static void WriteAll() // called when settings window closes
        {
            if (Find.CurrentMap != null)
                ActiveCircuitsBase.UpdateAllNets(Find.CurrentMap);
        }

        public static void DrawSettings(Rect rect)
        {
            Listing_Standard ls = new Listing_Standard(GameFont.Small);
            ls.ColumnWidth = rect.width -30f;
            ls.Begin(rect);

            ls.Gap();
            ls.CheckboxLabeled("Always display bad state circuits (may cause additional lag)", ref displayBadOnMain);

            ls.Gap();
            drawColorChoice(ls, ref justConduits,"Unconnected conduit (default 255, 255, 255)");
            drawColorChoice(ls, ref nothingConnected, "Nothing drawing power (default 255, 0, 255)");
            drawColorChoice(ls, ref noPower, "No power (default 255, 0, 0)");
            drawColorChoice(ls, ref drainingPower, "Batteries draining (default 250, 127, 114)");
            drawColorChoice(ls, ref hasPower, "Sufficient power (default 255, 235, 4)");
            drawColorChoice(ls, ref surplusPower, "Surplus power (default 0, 255, 0)");

            ls.Gap();
            ls.End();
        }

        public static void drawColorChoice(Listing_Standard ls, ref Color color, string description)
        {
            Rect colorRect = ls.GetRect(Text.LineHeight);
            Rect colorLabelRect = colorRect.LeftPartPixels(colorRect.width - rectWidth);
            Rect colorBoxRect = colorRect.RightPartPixels(rectWidth);
            colorBoxRect.height += 10f;
            colorBoxRect.y -= 3f;
            Widgets.Label(colorLabelRect, description);
            Widgets.DrawBoxSolid(colorBoxRect, color);
            ls.Gap();
            Rect colorSliders = ls.GetRect(Text.LineHeight);
            float sliderWidth = colorSliders.width / 3f;
            Rect colorSliderR = colorSliders.LeftPartPixels(sliderWidth - 5f);
            Rect colorSliderG = new Rect(colorSliders.xMin + sliderWidth + 5f, colorSliders.y, sliderWidth - 10f, colorSliders.height);
            Rect colorSliderB = colorSliders.RightPartPixels(sliderWidth - 5f);
            color.r = Widgets.HorizontalSlider(colorSliderR, color.r, 0, 1, false, "r: " + (color.r * 255).ToString("f0"), null, null, 1f / 255f);
            color.g = Widgets.HorizontalSlider(colorSliderG, color.g, 0, 1, false, "g: " + (color.g * 255).ToString("f0"), null, null, 1f / 255f);
            color.b = Widgets.HorizontalSlider(colorSliderB, color.b, 0, 1, false, "b: " + (color.b * 255).ToString("f0"), null, null, 1f / 255f);
            ls.Gap();
        }

    }

  
}
