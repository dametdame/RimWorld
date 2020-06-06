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
			var thingList = DefDatabase<ThingDef>.AllDefsListForReading;
			thingDic = new Dictionary<ThingDef, ResearchProjectDef>();
			researchDic = new Dictionary<ResearchProjectDef, List<ThingDef>>();
				foreach (ThingDef thing in thingList)
				{
					ResearchProjectDef rpd = GetBestResearchProject(thing);
					if (rpd != null)
					{
						CompProperties_DArcane comp = thing.GetCompProperties<CompProperties_DArcane>();
						if (comp == null)
							thing.comps.Add(new CompProperties_DArcane(rpd));

						List<ThingDef> things;

						thingDic.SetOrAdd(thing, rpd);
						if (researchDic.TryGetValue(rpd, out things))
							things.Add(thing);
						else
							researchDic.Add(rpd, new List<ThingDef> { thing });
					}
				}
		}

		public static ResearchProjectDef GetBestResearchProject(ThingDef thing)
		{
			ResearchProjectDef overrideRPD;
			if (GearAssigner.GetOverrideAssignment(thing, out overrideRPD))
				return overrideRPD;
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
				for (int i = (int)TechLevel.Archotech; i > 0; i--)
				{
					if (!strataDic.ContainsKey((TechLevel)i))
						continue;
					int numResearched = 0;
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
