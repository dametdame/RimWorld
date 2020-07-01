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

		public static ResearchProjectDef GetBestRPDForRecipe(RecipeDef recipe, bool ignoreLocked = false)
		{
			ThingDef thing = recipe.ProducedThingDef;
			if (thing == null)
			{
				return null;
			}
			ResearchProjectDef overrideRPD;
			if (GearAssigner.GetOverrideAssignment(thing, out overrideRPD))
				return overrideRPD;
			if (thing.category == ThingCategory.Building)
				return null;
			if (TechprintingSettings.weaponsApparelOnly && (!thing.IsWeapon && !thing.IsApparel) && !ignoreLocked && !TechprintingSettings.printAllItems)
				return null;

			if (recipe.researchPrerequisite != null) // direct prereq for this recipe
			{
				if (recipe.researchPrerequisite.techprintCount > 0 || TechprintingSettings.printAllItems || ignoreLocked)
					return recipe.researchPrerequisite;
			}

			if (recipe.researchPrerequisites != null && recipe.researchPrerequisites.Count > 0) // prerequisites list, never seen this but could be possible, not sure what's checked for
			{
				foreach (ResearchProjectDef prereq in recipe.researchPrerequisites)
				{
					if (prereq.techprintCount > 0)
						return prereq;
				}
				if (TechprintingSettings.printAllItems || ignoreLocked)
					return recipe.researchPrerequisites[0];
			}

			if (recipe.recipeUsers != null) // no direct prereqs, look at benches
			{
				foreach (ThingDef user in recipe.recipeUsers)
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
					foreach (ThingDef user in recipe.recipeUsers)
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
			return null;
		}
	}
}
