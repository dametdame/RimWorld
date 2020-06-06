using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DArcaneTechnology
{
    class SpecialThingFilterWorker_DUnresearchedApparel : SpecialThingFilterWorker
    {

		public override bool Matches(Thing t)
		{
			return t.def.IsApparel && Base.IsResearchLocked(t.def);
		}

		public override bool CanEverMatch(ThingDef def)
		{
			return def.IsApparel && Base.thingDic.ContainsKey(def);
		}
	}
}
