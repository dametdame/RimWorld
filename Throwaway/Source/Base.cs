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
		public static Dictionary<ThingDef, ResearchProjectDef>  thingDic = new Dictionary<ThingDef, ResearchProjectDef>();
		public static Dictionary<ResearchProjectDef, List<ThingDef>>  researchDic = new Dictionary<ResearchProjectDef, List<ThingDef>>();


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

			foreach (RecipeDef recipe in DefDatabase<RecipeDef>.AllDefsListForReading)
			{
				ResearchProjectDef rpd = DArcaneTechnology.Base.GetBestRPDForRecipe(recipe);
				if (rpd != null && recipe.ProducedThingDef != null)
				{
					ThingDef producedThing = recipe.ProducedThingDef;

					thingDic.SetOrAdd(producedThing, rpd);

					List<ThingDef> things;
					if (researchDic.TryGetValue(rpd, out things))
						things.Add(producedThing);
					else
						researchDic.Add(rpd, new List<ThingDef> { producedThing });
				}
			}

			GearAssigner.HardAssign(ref thingDic, ref researchDic);
			
			using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\unassignedgear.csv"))
			{
				var thingList = DefDatabase<ThingDef>.AllDefsListForReading;
				foreach (ThingDef thing in thingList)
				{
					ResearchProjectDef rpd = thingDic.TryGetValue(thing);
					if ((thing.IsWeapon || thing.IsApparel) && rpd == null)
					{
						file.WriteLine(thing.defName + "," + thing.label + "," + thing.modContentPack);
					}
				
				}
			}
			
		}



	}
}
