using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;

namespace DSkillPoints
{
    [StaticConstructorOnStartup]
    public static class Base
    {
        public const float panelAddedWidth = 185f;
        public static Color alternateRowColor = new Color(0.15f, 0.35f, 0.45f, 0.1f);

        static Base()
        {
            foreach(ThingDef td in DefDatabase<ThingDef>.AllDefs.Where(x => x.race != null && x.race.Humanlike))
            {
                td.comps.Add(new CompProperties_SkillPoints());
            }
        }

        public static void DoSkillLevelUp(Pawn pawn, SkillRecord sr)
        {
            
        }

        public static bool IsOnManual(Pawn p)
        {
            return true;
        }
    }
}
