using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using Verse;
/*
namespace DRimEditor.DetailView
{
	public static class ThingDefExtensions
    {
		// Token: 0x0600007C RID: 124 RVA: 0x00008B48 File Offset: 0x00006D48
		public static void RecacheRecipes(this ThingDef thingDef, Map map, bool validateBills)
		{
			bool flag = ThingDefExtensions._allRecipesCached == null;
			if (flag)
			{
				ThingDefExtensions._allRecipesCached = typeof(ThingDef).GetField("allRecipesCached", BindingFlags.Instance | BindingFlags.NonPublic);
			}
			ThingDefExtensions._allRecipesCached.SetValue(thingDef, null);
			bool flag2 = !validateBills || Current.ProgramState != ProgramState.MapInitializing;
			if (!flag2)
			{
				List<RecipeDef> recipes = thingDef.AllRecipes;
				IEnumerable<Building> buildings = map.listerBuildings.AllBuildingsColonistOfDef(thingDef);
				foreach (Building building in buildings)
				{
					IBillGiver iBillGiver = building as IBillGiver;
					bool flag3 = iBillGiver != null;
					if (flag3)
					{
						for (int i = 0; i < iBillGiver.BillStack.Count; i++)
						{
							Bill bill = iBillGiver.BillStack[i];
							bool flag4 = !recipes.Exists((RecipeDef r) => bill.recipe == r);
							if (flag4)
							{
								iBillGiver.BillStack.Delete(bill);
							}
						}
					}
				}
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00008C8C File Offset: 0x00006E8C
		public static bool IsFoodMachine(this ThingDef thingDef)
		{
			return typeof(Building_NutrientPasteDispenser).IsAssignableFrom(thingDef.thingClass);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00008CC0 File Offset: 0x00006EC0
		public static bool IsIngestible(this ThingDef thingDef)
		{
			return thingDef.ingestible != null;
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00008CDC File Offset: 0x00006EDC
		public static bool IsDrug(this ThingDef thingDef)
		{
			return thingDef.IsIngestible() && (thingDef.ingestible.drugCategory == DrugCategory.Hard || thingDef.ingestible.drugCategory == DrugCategory.Social);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00008D24 File Offset: 0x00006F24
		public static bool IsImplant(this ThingDef thingDef)
		{
			return DefDatabase<RecipeDef>.AllDefsListForReading.Exists((RecipeDef r) => r.addsHediff != null && r.IsIngredient(thingDef));
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00008D5C File Offset: 0x00006F5C
		public static RecipeDef GetImplantRecipeDef(this ThingDef thingDef)
		{
			return DefDatabase<RecipeDef>.AllDefsListForReading.Find((RecipeDef r) => r.addsHediff != null && r.IsIngredient(thingDef));
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00008D94 File Offset: 0x00006F94
		public static HediffDef GetImplantHediffDef(this ThingDef thingDef)
		{
			RecipeDef recipeDef = thingDef.GetImplantRecipeDef();
			return (recipeDef != null) ? recipeDef.addsHediff : null;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00008DBC File Offset: 0x00006FBC
		public static bool EverHasRecipes(this ThingDef thingDef)
		{
			return !thingDef.GetRecipesCurrent().NullOrEmpty<RecipeDef>() || !thingDef.GetRecipesUnlocked(ref ThingDefExtensions.nullDefs).NullOrEmpty<RecipeDef>();
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00008DF4 File Offset: 0x00006FF4
		public static bool EverHasRecipe(this ThingDef thingDef, RecipeDef recipeDef)
		{
			return thingDef.GetRecipesCurrent().Contains(recipeDef) || thingDef.GetRecipesUnlocked(ref ThingDefExtensions.nullDefs).Contains(recipeDef);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00008E28 File Offset: 0x00007028
		public static List<JoyGiverDef> GetJoyGiverDefsUsing(this ThingDef thingDef)
		{
			return (from def in DefDatabase<JoyGiverDef>.AllDefsListForReading
					where !def.thingDefs.NullOrEmpty<ThingDef>() && def.thingDefs.Contains(thingDef)
					select def).ToList<JoyGiverDef>();
		}

		// Token: 0x06000086 RID: 134 RVA: 0x00008E64 File Offset: 0x00007064
		public static List<RecipeDef> GetRecipesUnlocked(this ThingDef thingDef, ref List<Def> researchDefs)
		{
			List<RecipeDef> recipeDefs = new List<RecipeDef>();
			bool flag = researchDefs != null;
			if (flag)
			{
				researchDefs.Clear();
			}
			IEnumerable<RecipeDef> recipes = from r in DefDatabase<RecipeDef>.AllDefsListForReading
											 where r.researchPrerequisite != null && ((r.recipeUsers != null && r.recipeUsers.Contains(thingDef)) || (thingDef.recipes != null && thingDef.recipes.Contains(r)))
											 select r;
			recipeDefs.AddRangeUnique(recipes);
			return recipeDefs;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00008EC0 File Offset: 0x000070C0
		public static List<RecipeDef> GetRecipesCurrent(this ThingDef thingDef)
		{
			return thingDef.AllRecipes;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00008ED8 File Offset: 0x000070D8
		public static List<RecipeDef> GetRecipesAll(this ThingDef thingDef)
		{
			List<RecipeDef> recipeDefs = new List<RecipeDef>();
			recipeDefs.AddRangeUnique(thingDef.GetRecipesCurrent());
			recipeDefs.AddRangeUnique(thingDef.GetRecipesUnlocked(ref ThingDefExtensions.nullDefs));
			return recipeDefs;
		}

		// Token: 0x04000050 RID: 80
		internal static FieldInfo _allRecipesCached;

		// Token: 0x04000051 RID: 81
		public static List<Def> nullDefs = null;
	}
}
*/