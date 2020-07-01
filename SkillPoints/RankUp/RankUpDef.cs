using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace DSkillPoints
{
    public class RankUpDef : Def
    {
        public int pointCost = 1;
        public int maxRank = -1;
        public Type workerClass = typeof(RankUp_Stat);
		public StatDef associatedStat = null;
		public float statIncrease;

		public RankUpWorker Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (RankUpWorker)Activator.CreateInstance(this.workerClass, new object[] { this });
				}
				return this.workerInt;
			}
		}

		[Unsaved(false)]
        private RankUpWorker workerInt;
    }
}
