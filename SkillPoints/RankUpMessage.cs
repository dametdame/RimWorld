using DSkillPoints.Basic;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DSkillPoints
{
    public static class RankUpMessage
    {
        public static void DisplayMessage(Pawn pawn)
        {
            if (!SkillPointsSettings.showRankUpMessage)
                return;

            Messages.Message(new Message("SkillPointGainLabel".Translate(pawn.LabelShortCap, 0), MessageTypeDefOf.NeutralEvent));
        }
    }
}
