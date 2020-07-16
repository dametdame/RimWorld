using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;

namespace DThermodynamicsCore
{
    class ThermodynamicsSettings : ModSettings
    {
        public static float diffusionModifier = 1f;
        public static bool slowDiffuseWhileCarried = true;
        public static bool warmersSlowRot = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref diffusionModifier, "diffusionModifier", 1f);
            Scribe_Values.Look(ref slowDiffuseWhileCarried, "slowDiffuseWhileCarried", true);
            Scribe_Values.Look(ref warmersSlowRot, "warmersSlowRot", false);
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

            Rect diffusionRect = ls.GetRect(Text.LineHeight);
            Rect diffusionLabelRect = diffusionRect.LeftPartPixels(300);
            Rect diffusionSliderRect = diffusionRect.RightPartPixels(diffusionRect.width - 300);
            Widgets.Label(diffusionLabelRect, "Temperature diffusion rate (default 1.0)");
            diffusionModifier = Widgets.HorizontalSlider(diffusionSliderRect, diffusionModifier, 0, 5.0f, middleAlignment: false, label: diffusionModifier.ToString("f2"), leftAlignedLabel: null, rightAlignedLabel: null, roundTo: 0.05f);
            ls.Gap();

            ls.Gap();
            ls.CheckboxLabeled("Carried items change temperature very slowly", ref slowDiffuseWhileCarried);
            ls.Gap();
            ls.CheckboxLabeled("Warm storage slows rot", ref warmersSlowRot, "Things being warmed in storage will rot more slowly (equivalent to refrigeration)");

            ls.Gap();
            ls.End();
        }

    }
}
