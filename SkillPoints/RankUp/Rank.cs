using DSkillPoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DSkillPoints
{
    public class Rank
    {
        Pawn pawn;
        RankUpDef def;
        public int rank = 0;

        public int maxRank
        {
            get
            {
                return def.maxRank;
            }
        }

        public Rank(Pawn p, RankUpDef d)
        {
            this.def = d;
            this.pawn = p;
        }

        public bool CanRankUp()
        {
            return (maxRank < 0 || rank < maxRank);
        }

        public void RankUp()
        {
            if (!CanRankUp())
                return;
            def.Worker.RankUp();
        }
    }
}
