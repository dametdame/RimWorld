using System;
using UnityEngine;
using Verse;

namespace DRimEditor.DetailView
{
	public static class ResourceBank
	{
		// Token: 0x02000024 RID: 36
		[StaticConstructorOnStartup]
		public static class Icon
		{
			// Token: 0x04000073 RID: 115
			public static readonly Texture2D HelpMenuArrowUp = ContentFinder<Texture2D>.Get("UI/HelpMenu/ArrowUp", true);

			// Token: 0x04000074 RID: 116
			public static readonly Texture2D HelpMenuArrowDown = ContentFinder<Texture2D>.Get("UI/HelpMenu/ArrowDown", true);

			// Token: 0x04000075 RID: 117
			public static readonly Texture2D HelpMenuArrowRight = ContentFinder<Texture2D>.Get("UI/HelpMenu/ArrowRight", true);
		}

		// Token: 0x02000025 RID: 37
		[StaticConstructorOnStartup]
		public static class String
		{
			// Token: 0x04000076 RID: 118
			public static readonly string Finished = "Finished".Translate();

			// Token: 0x04000077 RID: 119
			public static readonly string InProgress = "InProgress".Translate();

			// Token: 0x04000078 RID: 120
			public static readonly string Locked = "Locked".Translate();

			// Token: 0x04000079 RID: 121
			public static readonly string Research = "Research".Translate();

			// Token: 0x0400007A RID: 122
			public static readonly string JumpToTopic = "RimEditJumpToTopic".Translate();

			// Token: 0x0400007B RID: 123
			public static readonly string BodyPartEfficiency = "BodyPartEfficiency".Translate();

			// Token: 0x0400007C RID: 124
			public static readonly string AutoHelpListCapacityModifiers = "AutoHelpListCapacityModifiers".Translate();

			// Token: 0x0400007D RID: 125
			public static readonly string MeleeWarmupTime = "MeleeWarmupTime".Translate();

			// Token: 0x0400007E RID: 126
			public static readonly string StatsReport_MeleeDamage = "StatsReport_MeleeDamage".Translate();

			// Token: 0x0400007F RID: 127
			public static readonly string WorkAmount = "WorkAmount".Translate();

			// Token: 0x04000080 RID: 128
			public static readonly string MinimumSkills = "MinimumSkills".Translate();

			// Token: 0x04000081 RID: 129
			public static readonly string Ingredients = "Ingredients".Translate();

			public static readonly string AutoHelpCategoryDefs = "AutoHelpCategoryDefs".Translate();

			// Token: 0x04000082 RID: 130
			public static readonly string AutoHelpCategoryItems = "AutoHelpCategoryItems".Translate();

			public static readonly string AutoHelpSubCategoryApparel = "AutoHelpSubCategoryApparel".Translate();

			// Token: 0x04000083 RID: 131
			public static readonly string AutoHelpSubCategoryDefs = "AutoHelpSubCategoryDefs".Translate();

			// Token: 0x04000084 RID: 132
			public static readonly string AutoHelpSubCategoryBodyParts = "AutoHelpSubCategoryBodyParts".Translate();

			// Token: 0x04000085 RID: 133
			public static readonly string AutoHelpSubCategoryDrugs = "AutoHelpSubCategoryDrugs".Translate();

			// Token: 0x04000086 RID: 134
			public static readonly string AutoHelpSubCategoryMeals = "AutoHelpSubCategoryMeals".Translate();

			// Token: 0x04000087 RID: 135
			public static readonly string AutoHelpSubCategoryWeapons = "AutoHelpSubCategoryWeapons".Translate();

			// Token: 0x04000088 RID: 136
			public static readonly string AutoHelpCategoryBuildings = "AutoHelpCategoryBuildings".Translate();

			// Token: 0x04000089 RID: 137
			public static readonly string AutoHelpSubCategorySpecial = "AutoHelpSubCategorySpecial".Translate();

			// Token: 0x0400008A RID: 138
			public static readonly string AutoHelpCategoryTerrain = "AutoHelpCategoryTerrain".Translate();

			// Token: 0x0400008B RID: 139
			public static readonly string AutoHelpSubCategoryTerrain = "AutoHelpSubCategoryTerrain".Translate();

			// Token: 0x0400008C RID: 140
			public static readonly string AutoHelpSubCategoryPlants = "AutoHelpSubCategoryPlants".Translate();

			// Token: 0x0400008D RID: 141
			public static readonly string AutoHelpCategoryFloraAndFauna = "AutoHelpCategoryFloraAndFauna".Translate();

			// Token: 0x0400008E RID: 142
			public static readonly string AutoHelpSubCategoryAnimals = "AutoHelpSubCategoryAnimals".Translate();

			// Token: 0x0400008F RID: 143
			public static readonly string AutoHelpSubCategoryMechanoids = "AutoHelpSubCategoryMechanoids".Translate();

			// Token: 0x04000090 RID: 144
			public static readonly string AutoHelpSubCategoryHumanoids = "AutoHelpSubCategoryHumanoids".Translate();

			// Token: 0x04000091 RID: 145
			public static readonly string AutoHelpSubCategoryBiomes = "AutoHelpSubCategoryBiomes".Translate();

			// Token: 0x04000092 RID: 146
			public static readonly string AutoHelpCategoryRecipes = "AutoHelpCategoryRecipes".Translate();

			// Token: 0x04000093 RID: 147
			public static readonly string AutoHelpCategoryResearch = "AutoHelpCategoryResearch".Translate();

			// Token: 0x04000094 RID: 148
			public static readonly string AutoHelpSubCategoryProjects = "AutoHelpSubCategoryProjects".Translate();

			// Token: 0x04000095 RID: 149
			public static readonly string AutoHelpLightRange = "AutoHelpLightRange".Translate();

			// Token: 0x04000096 RID: 150
			public static readonly string AutoHelpListAppearsInBiomes = "AutoHelpListAppearsInBiomes".Translate();

			// Token: 0x04000097 RID: 151
			public static readonly string AutoHelpListBiomeAnimals = "AutoHelpListBiomeAnimals".Translate();

			// Token: 0x04000098 RID: 152
			public static readonly string AutoHelpListBiomeDiseases = "AutoHelpListBiomeDiseases".Translate();

			// Token: 0x04000099 RID: 153
			public static readonly string AutoHelpListBiomePlants = "AutoHelpListBiomePlants".Translate();

			// Token: 0x0400009A RID: 154
			public static readonly string AutoHelpListBiomeTerrain = "AutoHelpListBiomeTerrain".Translate();

			// Token: 0x0400009B RID: 155
			public static readonly string AutoHelpListButcher = "AutoHelpListButcher".Translate();

			// Token: 0x0400009C RID: 156
			public static readonly string AutoHelpListCanBePlantedIn = "AutoHelpListCanBePlantedIn".Translate();

			// Token: 0x0400009D RID: 157
			public static readonly string AutoHelpListDisassemble = "AutoHelpListDisassemble".Translate();

			// Token: 0x0400009E RID: 158
			public static readonly string AutoHelpListFacilitiesAffected = "AutoHelpListFacilitiesAffected".Translate();

			// Token: 0x0400009F RID: 159
			public static readonly string AutoHelpListFertility = "AutoHelpListFertility".Translate();

			// Token: 0x040000A0 RID: 160
			public static readonly string AutoHelpListJoyActivities = "AutoHelpListJoyActivities".Translate();

			// Token: 0x040000A1 RID: 161
			public static readonly string AutoHelpListLifestages = "AutoHelpListLifestages".Translate();

			// Token: 0x040000A2 RID: 162
			public static readonly string AutoHelpListMilk = "AutoHelpListMilk".Translate();

			// Token: 0x040000A3 RID: 163
			public static readonly string AutoHelpListNutrition = "AutoHelpListNutrition".Translate();

			// Token: 0x040000A4 RID: 164
			public static readonly string AutoHelpListNutritionPlant = "AutoHelpListNutritionPlant".Translate();

			// Token: 0x040000A5 RID: 165
			public static readonly string AutoHelpListPathCost = "AutoHelpListPathCost".Translate();

			// Token: 0x040000A6 RID: 166
			public static readonly string AutoHelpListPlantsIn = "AutoHelpListPlantsIn".Translate();

			// Token: 0x040000A7 RID: 167
			public static readonly string AutoHelpListPlantsUnlocked = "AutoHelpListPlantsUnlocked".Translate();

			// Token: 0x040000A8 RID: 168
			public static readonly string AutoHelpListPlantYield = "AutoHelpListPlantYield".Translate();

			// Token: 0x040000A9 RID: 169
			public static readonly string AutoHelpListRecipeProducts = "AutoHelpListRecipeProducts".Translate();

			// Token: 0x040000AA RID: 170
			public static readonly string AutoHelpListRecipes = "AutoHelpListRecipes".Translate();

			// Token: 0x040000AB RID: 171
			public static readonly string AutoHelpListRecipesOnThings = "AutoHelpListRecipesOnThings".Translate();

			// Token: 0x040000AC RID: 172
			public static readonly string AutoHelpListRecipesOnThingsUnlocked = "AutoHelpListRecipesOnThingsUnlocked".Translate();

			// Token: 0x040000AD RID: 173
			public static readonly string AutoHelpListRecipesUnlocked = "AutoHelpListRecipesUnlocked".Translate();

			// Token: 0x040000AE RID: 174
			public static readonly string AutoHelpListReproduction = "AutoHelpListReproduction".Translate();

			// Token: 0x040000AF RID: 175
			public static readonly string AutoHelpListResearchBy = "AutoHelpListResearchBy".Translate();

			// Token: 0x040000B0 RID: 176
			public static readonly string AutoHelpListResearchLeadsTo = "AutoHelpListResearchLeadsTo".Translate();

			// Token: 0x040000B1 RID: 177
			public static readonly string AutoHelpListResearchRequired = "AutoHelpListResearchRequired".Translate();

			// Token: 0x040000B2 RID: 178
			public static readonly string AutoHelpListShear = "AutoHelpListShear".Translate();

			// Token: 0x040000B3 RID: 179
			public static readonly string AutoHelpListStatOffsets = "AutoHelpListStatOffsets".Translate();

			// Token: 0x040000B4 RID: 180
			public static readonly string AutoHelpListThingsUnlocked = "AutoHelpListThingsUnlocked".Translate();

			// Token: 0x040000B5 RID: 181
			public static readonly string AutoHelpListTrainable = "AutoHelpListTrainable".Translate();

			// Token: 0x040000B6 RID: 182
			public static readonly string AutoHelpCost = "AutoHelpCost".Translate();

			// Token: 0x040000B7 RID: 183
			public static readonly string AutoHelpDiet = "AutoHelpDiet".Translate();

			// Token: 0x040000B8 RID: 184
			public static readonly string AutoHelpEfficiency = "AutoHelpEfficiency".Translate();

			// Token: 0x040000B9 RID: 185
			public static readonly string AutoHelpFacilityStats = "AutoHelpFacilityStats".Translate();

			// Token: 0x040000BA RID: 186
			public static readonly string AutoHelpGenerates = "AutoHelpGenerates".Translate();

			// Token: 0x040000BB RID: 187
			public static readonly string AutoHelpGestationPeriod = "AutoHelpGestationPeriod".Translate();

			// Token: 0x040000BC RID: 188
			public static readonly string AutoHelpGrowDays = "AutoHelpGrowDays".Translate();

			// Token: 0x040000BD RID: 189
			public static readonly string AutoHelpHealthScale = "AutoHelpHealthScale".Translate();

			// Token: 0x040000BE RID: 190
			public static readonly string AutoHelpIdlePower = "AutoHelpIdlePower".Translate();

			// Token: 0x040000BF RID: 191
			public static readonly string AutoHelpIntelligence = "AutoHelpIntelligence".Translate();

			// Token: 0x040000C0 RID: 192
			public static readonly string AutoHelpJoyKind = "AutoHelpJoyKind".Translate();

			// Token: 0x040000C1 RID: 193
			public static readonly string AutoHelpJoySkill = "AutoHelpJoySkill".Translate();

			// Token: 0x040000C2 RID: 194
			public static readonly string AutoHelpLifeExpectancy = "AutoHelpLifeExpectancy".Translate();

			// Token: 0x040000C3 RID: 195
			public static readonly string AutoHelpLitterSize = "AutoHelpLitterSize".Translate();

			// Token: 0x040000C4 RID: 196
			public static readonly string AutoHelpMaximumAffected = "AutoHelpMaximumAffected".Translate();

			// Token: 0x040000C5 RID: 197
			public static readonly string AutoHelpMaximumParticipants = "AutoHelpMaximumParticipants".Translate();

			// Token: 0x040000C6 RID: 198
			public static readonly string AutoHelpMinFertility = "AutoHelpMinFertility".Translate();

			// Token: 0x040000C7 RID: 199
			public static readonly string AutoHelpPower = "AutoHelpPower".Translate();

			// Token: 0x040000C8 RID: 200
			public static readonly string AutoHelpRequired = "AutoHelpRequired".Translate();

			// Token: 0x040000C9 RID: 201
			public static readonly string AutoHelpStores = "AutoHelpStores".Translate();

			// Token: 0x040000CA RID: 202
			public static readonly string AutoHelpSurgeryFixOrReplace = "AutoHelpSurgeryFixOrReplace".Translate();

			// Token: 0x040000CB RID: 203
			public static readonly string AutoHelpTotalCost = "AutoHelpTotalCost".Translate();
		}
	}
}
