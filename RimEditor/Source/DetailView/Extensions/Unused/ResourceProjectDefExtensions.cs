using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using DRimEditor.DetailView;
/*
namespace DRimEditor.DetailView
{
	// Token: 0x02000017 RID: 23
	[StaticConstructorOnStartup]
	public static class ResearchProjectDefExtensions
	{
		// Token: 0x06000094 RID: 148 RVA: 0x00009600 File Offset: 0x00007800
		public static List<ResearchProjectDef> ExclusiveDescendants(this ResearchProjectDef research)
		{
			List<ResearchProjectDef> descendants = new List<ResearchProjectDef>();
			Queue<ResearchProjectDef> queue = new Queue<ResearchProjectDef>(from res in DefDatabase<ResearchProjectDef>.AllDefsListForReading
																			where res.prerequisites.Contains(research)
																			select res);
			while (queue.Count > 0)
			{
				ResearchProjectDef current = queue.Dequeue();
				bool flag = !descendants.Contains(current);
				if (flag)
				{
					descendants.Add(current);
					IEnumerable<ResearchProjectDef> allDefsListForReading = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
					Func<ResearchProjectDef, bool> predicate = ((ResearchProjectDef res) => res.prerequisites.Contains(current));
					foreach (ResearchProjectDef descendant in allDefsListForReading.Where(predicate))
					{
						queue.Enqueue(descendant);
					}
				}
			}
			return descendants;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00009704 File Offset: 0x00007904
		public static List<ResearchProjectDef> GetPrerequisitesRecursive(this ResearchProjectDef research)
		{
			List<ResearchProjectDef> result = new List<ResearchProjectDef>();
			bool flag = research.prerequisites.NullOrEmpty<ResearchProjectDef>();
			List<ResearchProjectDef> result2;
			if (flag)
			{
				result2 = result;
			}
			else
			{
				Stack<ResearchProjectDef> stack = new Stack<ResearchProjectDef>(from parent in research.prerequisites
																				where parent != research
																				select parent);
				while (stack.Count > 0)
				{
					ResearchProjectDef parent2 = stack.Pop();
					result.Add(parent2);
					bool flag2 = !parent2.prerequisites.NullOrEmpty<ResearchProjectDef>();
					if (flag2)
					{
						foreach (ResearchProjectDef grandparent in parent2.prerequisites)
						{
							bool flag3 = grandparent != parent2;
							if (flag3)
							{
								stack.Push(grandparent);
							}
						}
					}
				}
				result2 = result.Distinct<ResearchProjectDef>().ToList<ResearchProjectDef>();
			}
			return result2;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00009814 File Offset: 0x00007A14
		public static List<Pair<Def, string>> GetUnlockDefsAndDescs(this ResearchProjectDef research)
		{
			bool flag = ResearchProjectDefExtensions._unlocksCache.ContainsKey(research.shortHash);
			List<Pair<Def, string>> result;
			if (flag)
			{
				result = ResearchProjectDefExtensions._unlocksCache[research.shortHash];
			}
			else
			{
				List<Pair<Def, string>> unlocks = new List<Pair<Def, string>>();
				List<ThingDef> dump = new List<ThingDef>();
				unlocks.AddRange(from d in research.GetThingsUnlocked()
								 where d.IconTexture() != null
								 select new Pair<Def, string>(d, "AllowsBuildingX".Translate(d.LabelCap)));
				unlocks.AddRange(from d in research.GetTerrainUnlocked()
								 where d.IconTexture() != null
								 select new Pair<Def, string>(d, "AllowsBuildingX".Translate(d.LabelCap)));
				unlocks.AddRange(from d in research.GetRecipesUnlocked(ref dump)
								 where d.IconTexture() != null
								 select new Pair<Def, string>(d, "AllowsCraftingX".Translate(d.LabelCap)));
				string sowTags = string.Join(" and ", research.GetSowTagsUnlocked(ref dump).ToArray());
				unlocks.AddRange(from d in dump
								 where d.IconTexture() != null
								 select new Pair<Def, string>(d, "AllowsSowingXinY".Translate(d.LabelCap, sowTags)));
				ResearchProjectDefExtensions._unlocksCache.Add(research.shortHash, unlocks);
				result = unlocks;
			}
			return result;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000099D4 File Offset: 0x00007BD4
		public static List<ThingDef> GetThingsUnlocked(this ResearchProjectDef researchProjectDef)
		{
			List<ThingDef> thingsOn = new List<ThingDef>();
			List<ThingDef> researchThings = DefDatabase<ThingDef>.AllDefsListForReading.Where(delegate (ThingDef t)
			{
				List<Def> r = t.GetResearchRequirements();
				return r != null && r.Contains(researchProjectDef);
			}).ToList<ThingDef>();
			bool flag = !researchThings.NullOrEmpty<ThingDef>();
			if (flag)
			{
				thingsOn.AddRangeUnique(researchThings);
			}
			return thingsOn;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00009A30 File Offset: 0x00007C30
		public static List<TerrainDef> GetTerrainUnlocked(this ResearchProjectDef researchProjectDef)
		{
			List<TerrainDef> thingsOn = new List<TerrainDef>();
			List<TerrainDef> researchThings = DefDatabase<TerrainDef>.AllDefsListForReading.Where(delegate (TerrainDef t)
			{
				List<Def> r = t.GetResearchRequirements();
				return r != null && r.Contains(researchProjectDef);
			}).ToList<TerrainDef>();
			bool flag = !researchThings.NullOrEmpty<TerrainDef>();
			if (flag)
			{
				thingsOn.AddRangeUnique(researchThings);
			}
			return thingsOn;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00009A8C File Offset: 0x00007C8C
		public static List<RecipeDef> GetRecipesUnlocked(this ResearchProjectDef researchProjectDef, ref List<ThingDef> thingDefs)
		{
			List<RecipeDef> recipes = new List<RecipeDef>();
			bool flag = thingDefs != null;
			if (flag)
			{
				thingDefs.Clear();
			}
			List<RecipeDef> researchRecipes = (from d in DefDatabase<RecipeDef>.AllDefsListForReading
											   where d.researchPrerequisite == researchProjectDef
											   select d).ToList<RecipeDef>();
			bool flag2 = !researchRecipes.NullOrEmpty<RecipeDef>();
			if (flag2)
			{
				recipes.AddRangeUnique(researchRecipes);
			}
			bool flag3 = thingDefs != null;
			if (flag3)
			{
				using (List<RecipeDef>.Enumerator enumerator = recipes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RecipeDef r = enumerator.Current;
						bool flag4 = !r.recipeUsers.NullOrEmpty<ThingDef>();
						if (flag4)
						{
							thingDefs.AddRangeUnique(r.recipeUsers);
						}
						List<ThingDef> recipeThings = (from d in DefDatabase<ThingDef>.AllDefsListForReading
													   where !d.recipes.NullOrEmpty<RecipeDef>() && d.recipes.Contains(r)
													   select d).ToList<ThingDef>();
						bool flag5 = !recipeThings.NullOrEmpty<ThingDef>();
						if (flag5)
						{
							thingDefs.AddRangeUnique(recipeThings);
						}
					}
				}
			}
			return recipes;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00009BC8 File Offset: 0x00007DC8
		public static List<string> GetSowTagsUnlocked(this ResearchProjectDef researchProjectDef, ref List<ThingDef> thingDefs)
		{
			List<string> sowTags = new List<string>();
			bool flag = thingDefs != null;
			if (flag)
			{
				thingDefs.Clear();
			}
			List<ThingDef> researchPlants = (from d in DefDatabase<ThingDef>.AllDefsListForReading
											 where d.plant != null && !d.plant.sowResearchPrerequisites.NullOrEmpty<ResearchProjectDef>() && d.plant.sowResearchPrerequisites.Contains(researchProjectDef)
											 select d).ToList<ThingDef>();
			bool flag2 = !researchPlants.NullOrEmpty<ThingDef>();
			if (flag2)
			{
				foreach (ThingDef plant in researchPlants)
				{
					sowTags.AddRangeUnique(plant.plant.sowTags);
				}
				bool flag3 = thingDefs != null;
				if (flag3)
				{
					thingDefs.AddRangeUnique(researchPlants);
				}
			}
			return sowTags;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00009C9C File Offset: 0x00007E9C
		public static List<Def> GetResearchRequirements(this ResearchProjectDef researchProjectDef)
		{
			List<Def> researchDefs = new List<Def>();
			bool flag = researchProjectDef.prerequisites != null;
			if (flag)
			{
				researchDefs.AddRangeUnique(researchProjectDef.prerequisites.ConvertAll<Def>((ResearchProjectDef def) => def));
			}
			return researchDefs;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00009CF8 File Offset: 0x00007EF8
		public static List<Def> GetResearchUnlocked(this ResearchProjectDef researchProjectDef)
		{
			List<Def> researchDefs = new List<Def>();
			researchDefs.AddRangeUnique((from rd in DefDatabase<ResearchProjectDef>.AllDefsListForReading
										 where !rd.prerequisites.NullOrEmpty<ResearchProjectDef>() && rd.prerequisites.Contains(researchProjectDef)
										 select rd).ToList<ResearchProjectDef>().ConvertAll<Def>((ResearchProjectDef def) => def));
			return researchDefs;
		}

		// Token: 0x04000052 RID: 82
		private static Dictionary<ushort, List<Pair<Def, string>>> _unlocksCache = new Dictionary<ushort, List<Pair<Def, string>>>();
	}
}
*/