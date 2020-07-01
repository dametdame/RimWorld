using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace DSkillPoints.Basic
{
    public class SkillPointsSettings : ModSettings
    {

        public static Vector2 scrollPos = new Vector2();
        public static bool highlightAlternate = false;
        public static bool showRankUpMessage = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref highlightAlternate, "highlightAlternate", false);
            Scribe_Values.Look(ref showRankUpMessage, "showRankUpMessage", true);
            base.ExposeData();
        }

        public static void WriteAll()
        {
            
        }

        public static void DrawSettings(Rect rect)
        {
            Listing_Standard ls = new Listing_Standard(GameFont.Small);

            //Rect contents = new Rect(rect.x, rect.y, rect.width - 30f, height);
            //Widgets.BeginScrollView(rect, ref scrollPos, contents, true);
            //ls.ColumnWidth = contents.width * 2.0f / 3.0f;
            //ls.Begin(contents);
            ls.Begin(rect);
            ls.Gap();

            ls.CheckboxLabeled("Show message on rank up", ref showRankUpMessage);
            ls.Gap();

            ls.CheckboxLabeled("Hightlight alternate lines in skill display", ref highlightAlternate);
            ls.Gap();

            ls.End();
            //Widgets.EndScrollView();
        }

       
    }
}
