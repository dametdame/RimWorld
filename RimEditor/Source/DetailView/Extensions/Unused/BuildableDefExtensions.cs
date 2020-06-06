using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
/*
namespace DRimEditor.DetailView
{
	// Token: 0x02000015 RID: 21
	[StaticConstructorOnStartup]
	public static class BuildableDefExtensions
	{
		// Token: 0x0600008A RID: 138 RVA: 0x00008F18 File Offset: 0x00007118
		public static bool HasResearchRequirement(this BuildableDef buildableDef)
		{
			bool result;
			if (buildableDef.researchPrerequisites != null)
			{
				result = buildableDef.researchPrerequisites.Any((ResearchProjectDef def) => def != null);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00008F6C File Offset: 0x0000716C
		public static List<Def> GetResearchRequirements(this BuildableDef buildableDef)
		{
			List<Def> researchDefs = new List<Def>();
			bool flag = buildableDef.researchPrerequisites != null;
			if (flag)
			{
				researchDefs.AddRangeUnique(buildableDef.researchPrerequisites.ConvertAll<Def>((ResearchProjectDef def) => def));
			}
			return researchDefs;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00008FC8 File Offset: 0x000071C8
		public static List<RecipeDef> GetRecipeDefs(this BuildableDef buildableDef)
		{
			return DefDatabase<RecipeDef>.AllDefsListForReading.Where(delegate (RecipeDef r)
			{
				List<ThingDefCountClass> products = r.products;
				Predicate<ThingDefCountClass> predicate = ((ThingDefCountClass tc) => tc.thingDef == buildableDef as ThingDef);
				return products.Any(predicate);
			}).ToList<RecipeDef>();
		}
	}
}
*/