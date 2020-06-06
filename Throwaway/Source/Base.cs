using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using DArcaneTechnology;

namespace Throwaway
{
    [StaticConstructorOnStartup]
    public static class Base
    {

        static Base()
        {
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\researchprojects.csv"))
			{
				ThingCategoryDef bp = DefDatabase<ThingCategoryDef>.GetNamed("BuildingsProduction");
				foreach(ResearchProjectDef rpd in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
				{
					if (rpd.UnlockedDefs.Any(delegate (Def d) { ThingDef td = d as ThingDef; if (td != null) 
							return (td.IsWeapon || td.IsApparel || (td.building != null && (td.building.buildingTags != null && td.building.buildingTags.Contains("Production") || td.thingCategories != null && td.thingCategories.Contains(bp)))); return false; }))
						file.WriteLine(rpd.defName + "," + rpd.label + "," + rpd.modContentPack);
				}
			}

			using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\unassignedgear.csv"))
			{
				var thingList = DefDatabase<ThingDef>.AllDefsListForReading;
				foreach (ThingDef thing in thingList)
				{
					ResearchProjectDef rpd = GetBestResearchProject(thing);
					if ((thing.IsWeapon || thing.IsApparel) && rpd == null)
					{
						file.WriteLine(thing.defName + "," + thing.label + "," + thing.modContentPack);
					}
				
				}
			}

		}


		public static ResearchProjectDef GetBestResearchProject(ThingDef thing)
		{
			if (thing.category == ThingCategory.Building || !(thing.IsWeapon || thing.IsApparel))
				return null;

			var rm = thing.recipeMaker;
			if (rm != null)
			{
				if (rm.researchPrerequisite != null)
				{
					return rm.researchPrerequisite;
				}
				if (rm.researchPrerequisites != null && rm.researchPrerequisites.Count > 0)
				{
					return rm.researchPrerequisites[0];
				}
				if (rm.recipeUsers != null)
				{
					float lowestSpeed = 99999f;
					TechLevel lowestTech = TechLevel.Archotech;
					ThingDef bestThing = null;
					foreach (ThingDef user in rm.recipeUsers)
					{
						if (user.researchPrerequisites != null && user.researchPrerequisites.Count > 0)
						{
							float workRate = user.GetStatValueAbstract(StatDefOf.WorkTableWorkSpeedFactor);
							if (workRate <= lowestSpeed && user.researchPrerequisites[0].techLevel <= lowestTech)
							{
								bestThing = user;
								lowestSpeed = workRate;
								lowestTech = user.researchPrerequisites[0].techLevel;
							}
						}
						else
							return null;
					}
					if (bestThing != null)
						return bestThing.researchPrerequisites[0];
				}
			}
			ResearchProjectDef rpd;
			if (GearAssigner.GetHardAssignment(thing, out rpd))
				return rpd;
			return null;
		}

	}
}
