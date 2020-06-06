using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DArcaneTechnology
{
    class SpecialThingFilterWorker_DResearchedWeapons : SpecialThingFilterWorker
    {
		public override bool Matches(Thing t)
		{
			return t.def.IsWeapon && !Base.IsResearchLocked(t.def);
		}

		public override bool CanEverMatch(ThingDef def)
		{
			return def.IsWeapon && Base.thingDic.ContainsKey(def);
		}
	}
}
