using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSkillPoints
{
    public abstract class RankUpWorker
    {
        public RankUpDef def;

        public RankUpWorker(RankUpDef d)
        {
            def = d;
        }

        public abstract void RankUp();
    }
}
