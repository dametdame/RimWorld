using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;
using Verse.Noise;


namespace DHighVoltage
{
    class HighVoltageSettings : ModSettings
    {

        public override void ExposeData()
        {
            base.ExposeData();
        }

        public static void WriteAll() // called when settings window closes
        {

        }

        public static void DrawSettings(Rect rect)
        {
            Listing_Standard ls = new Listing_Standard(GameFont.Small);
            ls.ColumnWidth = rect.width - 30f;
            ls.Begin(rect);
            ls.Gap();

            ls.Gap();
            ls.End();
        }

    }


}
