using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace DTechprinting
{
    public static class ThingDefHelper
    {
		public static bool ThingIsPrintable(ThingDef thing, bool ignoreLocked = false)
		{
			return (GetBestResearchProject(thing, ignoreLocked) != null);
		}


		public static ResearchProjectDef GetBestResearchProject(ThingDef thing, bool ignoreLocked = false)
		{
			if (thing.category == ThingCategory.Building)
				return null;
			if (TechprintingSettings.weaponsApparelOnly && (!thing.IsWeapon && !thing.IsApparel) && !ignoreLocked && !TechprintingSettings.printAllItems)
				return null;
			var rm = thing.recipeMaker;
			if (rm != null)
			{
				if (rm.researchPrerequisite != null)
				{
					if (rm.researchPrerequisite.techprintCount > 0 || TechprintingSettings.printAllItems || ignoreLocked)
						return rm.researchPrerequisite;
				}
				if (rm.researchPrerequisites != null && rm.researchPrerequisites.Count > 0)
				{
					foreach (ResearchProjectDef prereq in rm.researchPrerequisites)
					{
						if (prereq.techprintCount > 0)
							return prereq;
					}
					if (TechprintingSettings.printAllItems || ignoreLocked)
						return rm.researchPrerequisites[0];
				}
				if (rm.recipeUsers != null)
				{
					foreach (ThingDef user in rm.recipeUsers)
					{
						if (user.researchPrerequisites != null && user.researchPrerequisites.Count > 0)
						{
							foreach (ResearchProjectDef prereq in user.researchPrerequisites)
							{
								if (prereq.techprintCount > 0)
									return prereq;
							}
						}
					}
					if (TechprintingSettings.printAllItems || ignoreLocked)
					{
						float lowestSpeed = 99999f;
						ThingDef bestThing = null;
						foreach (ThingDef user in rm.recipeUsers)
						{
							if (user.researchPrerequisites != null && user.researchPrerequisites.Count > 0)
							{
								float workRate = user.GetStatValueAbstract(StatDefOf.WorkTableWorkSpeedFactor);
								if (workRate < lowestSpeed)
								{
									bestThing = user;
									lowestSpeed = workRate;
								}
							}
						}
						if (bestThing != null)
							return bestThing.researchPrerequisites[0];
					}
				}
			}
			ResearchProjectDef rpd;
			if (GearAssigner.GetHardAssignment(thing, out rpd))
				return rpd;
			return null;
		}

	}
}
