using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace DSkillPoints
{
    public class ITab_Skills : ITab
    {
        public const float margin = 5f;
        public const float bigMargin = 17f;
        public const float rankInfoWidth = Base.panelAddedWidth;

        private float cachedRankupWidth = -1f;
        private float cachedRankupHeight = -1f;

        public float rankupWidth
        {
            get
            {
                if (cachedRankupWidth < 0f)
                {
                    // fill
                }
                return cachedRankupWidth;
            }
        }

        public float rankupHeight
        {
            get
            {
                if (cachedRankupHeight < 0f)
                {
                    // fill
                }
                return cachedRankupHeight;
            }
        }

        public ITab_Skills()
        {
            this.size = new Vector2(500f, 500f);
            this.labelKey = "SkillPointsTab";
        }

        public override bool IsVisible
        {
            get
            {
                return this.SelPawn.TryGetComp<CompSkillPoints>() != null;
            }
        }

        public static void DrawRankInfo(Rect rect, Pawn pawn)
        {
            CompSkillPoints spcomp = pawn.TryGetComp<CompSkillPoints>();
            if (spcomp == null)
                return;
            Text.Font = GameFont.Medium;
            Rect rankRect = new Rect(rect.xMin, rect.yMin, rect.width, Text.LineHeight);
            Widgets.Label(rankRect, "Rank: 99");
            Text.Font = GameFont.Small;
            Rect xpRect = new Rect(rect.xMin, rankRect.yMax + margin, rect.width, Text.LineHeight);
            Widgets.Label(xpRect, "Next point: 999999 / 999999");
            if (Current.Game != null && pawn.IsColonist)
            {
                Rect pointsrect = new Rect(rect.xMin, xpRect.yMax + margin, rect.width, Text.LineHeight);
                Widgets.Label(pointsrect, "Unspent points: 99");
            }
        }

        protected override void FillTab()
        {
            Pawn pawn = this.SelPawn;
            CompSkillPoints spcomp = pawn.TryGetComp<CompSkillPoints>();
            if (spcomp == null)
                return;
            Rect masterRect = new Rect(bigMargin, bigMargin, this.size.x - bigMargin * 2, this.size.y - bigMargin * 2);
            GUI.BeginGroup(masterRect);
            Rect rankInfoRect = new Rect(0, 0, rankInfoWidth, 100f);
            DrawRankInfo(rankInfoRect, pawn);
            GUI.EndGroup();
        }

    }
}
