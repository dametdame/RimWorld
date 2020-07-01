using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DArcaneTechnology
{
    [StaticConstructorOnStartup]
    public static class Base
    {
		public static Dictionary<ThingDef, ResearchProjectDef> thingDic = null;
		public static Dictionary<ResearchProjectDef, List<ThingDef>> researchDic = null;

		public static Dictionary<TechLevel, List<ResearchProjectDef>> strataDic = new Dictionary<TechLevel, List<ResearchProjectDef>>();

		public static TechLevel playerTechLevel
		{
			get
			{
				if (cachedTechLevel == TechLevel.Undefined)
				{
					cachedTechLevel = GetPlayerTech();
				}
				return cachedTechLevel;
			}
			set
			{
				bool flag = false;
				if (cachedTechLevel == TechLevel.Undefined)
				{
					flag = true;
				}
				cachedTechLevel = value;
				if (flag)
				{
					// This is the first set, but I don't remember what I intended to do here
				}
			}
		}
		private static TechLevel cachedTechLevel = TechLevel.Undefined;

		static Base()
        {
			
        }

		public static void Initialize()
		{
			foreach (ResearchProjectDef rpd in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
			{
				if (!strataDic.ContainsKey(rpd.techLevel))
				{
					strataDic.Add(rpd.techLevel, new List<ResearchProjectDef>());
				}
				if (!strataDic[rpd.techLevel].Contains(rpd))
					strataDic[rpd.techLevel].Add(rpd);
			}
			MakeDictionaries();
		}

		public static void MakeDictionaries()
		{
			thingDic = new Dictionary<ThingDef, ResearchProjectDef>();
			researchDic = new Dictionary<ResearchProjectDef, List<ThingDef>>();

			foreach (RecipeDef recipe in DefDatabase<RecipeDef>.AllDefsListForReading)
			{
				ResearchProjectDef rpd = GetBestRPDForRecipe(recipe);
				if (rpd != null && recipe.ProducedThingDef != null)
				{
					ThingDef producedThing = recipe.ProducedThingDef;

					CompProperties_DArcane comp = producedThing.GetCompProperties<CompProperties_DArcane>();
					if (comp == null)
						producedThing.comps.Add(new CompProperties_DArcane(rpd));				
					
					thingDic.SetOrAdd(producedThing, rpd);

					List<ThingDef> things;
					if (researchDic.TryGetValue(rpd, out things))
						things.Add(producedThing);
					else
						researchDic.Add(rpd, new List<ThingDef> { producedThing });				
				}
			}

			GearAssigner.HardAssign(ref thingDic, ref researchDic);
			GearAssigner.OverrideAssign(ref thingDic, ref researchDic);
		}

		public static ResearchProjectDef GetBestRPDForRecipe(RecipeDef recipe)
		{
			ThingDef thing = recipe.ProducedThingDef;
			if (thing == null)
			{
				return null;
			}
			ResearchProjectDef overrideRPD;
			if (GearAssigner.GetOverrideAssignment(thing, out overrideRPD))
				return overrideRPD;
			if (thing.category == ThingCategory.Building || !(thing.IsWeapon || thing.IsApparel))
				return null;

			if (recipe.researchPrerequisite != null)
			{
				return recipe.researchPrerequisite;
			}
			if (recipe.researchPrerequisites != null && recipe.researchPrerequisites.Count > 0)
			{
				return recipe.researchPrerequisites[0];
			}
			if (recipe.recipeUsers != null)
			{
				float lowestSpeed = 99999f;
				TechLevel lowestTech = TechLevel.Archotech;
				ThingDef bestThing = null;
				foreach (ThingDef user in recipe.recipeUsers)
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
			return null;
		}

		public static bool InLockedTechRange(ResearchProjectDef rpd)
		{
			if (ArcaneTechnologySettings.restrictOnTechLevel)
				return (rpd.techLevel > playerTechLevel);
			return (rpd.techLevel >= ArcaneTechnologySettings.minToRestrict);
		}

		public static bool Locked(ResearchProjectDef rpd)
		{
			if (rpd != null && DefDatabase<ResearchProjectDef>.AllDefs.Contains(rpd) && !GearAssigner.ProjectIsExempt(rpd) && InLockedTechRange(rpd))
			{
				if (!rpd.IsFinished || ArcaneTechnologySettings.evenResearched)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsResearchLocked(ThingDef thingDef, Pawn pawn = null)
		{
			if (pawn != null && !pawn.IsColonist)
				return false;
			ResearchProjectDef rpd;
			if (thingDic.TryGetValue(thingDef, out rpd) && Locked(rpd))
			{
				return true;
			}
			return false;
		}

		public static TechLevel GetPlayerTech()
		{
			if (ArcaneTechnologySettings.useHighestResearched)
			{
				for (int i = (int)TechLevel.Archotech; i > 0; i--)
				{
					if (!strataDic.ContainsKey((TechLevel)i))
						continue;
					foreach (ResearchProjectDef rpd in strataDic[(TechLevel)i])
					{
						if (rpd.IsFinished)
							return (TechLevel)i;
					}
				}
				return TechLevel.Animal;
			}
			else if (ArcaneTechnologySettings.usePercentResearched)
			{
				int numResearched = 0;
				for (int i = (int)TechLevel.Archotech; i > 0; i--)
				{
					if (!strataDic.ContainsKey((TechLevel)i))
						continue;					
					foreach (ResearchProjectDef rpd in strataDic[(TechLevel)i])
					{
						if (rpd.IsFinished)
							numResearched++;
					}
					if ((float)numResearched / (float)strataDic[(TechLevel)i].Count >= ArcaneTechnologySettings.percentResearchNeeded)
						return (TechLevel)i;
				}
				return TechLevel.Animal;
			}
			else // (IgnoranceSettings.useActualTechLevel) or fixedTechRange
			{
				return Faction.OfPlayer.def.techLevel;
			}
		}

	}
}
