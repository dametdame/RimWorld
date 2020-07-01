using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using DHotMeals.Comps;
using DThermodynamicsCore.Comps;

namespace DHotMeals
{

  
    public enum MealTempTypes
    {
        None,
        HotMeal,
        ColdMeal,
        HotDrink,
        ColdDrink,
        RoomTempMeal,
        NonPerishable,
        RawTasty,
        RawResource
    }

    public static class MealAssigner
    {

        public static Dictionary<string, MealTempTypes> fixedTypes = new Dictionary<string, MealTempTypes>(); // read by patch operations

        public static List<ThingCategoryDef> hotMealCats = new List<ThingCategoryDef> { };
        public static List<ThingCategoryDef> coldMealCats = new List<ThingCategoryDef> { };
        public static List<ThingCategoryDef> hotDrinkCats = new List<ThingCategoryDef> { };
        public static List<ThingCategoryDef> coldDrinkCats = new List<ThingCategoryDef> { };
        public static List<ThingCategoryDef> RTMealCats = new List<ThingCategoryDef> { };
        public static List<ThingCategoryDef> nonperishCats = new List<ThingCategoryDef> { };
        public static List<ThingCategoryDef> rawTastyCats = new List<ThingCategoryDef> { };


        public static List<string> excludedCatDefs = new List<string>() { // don't hijack these categories
            "RC2_GrainsRaw",
            "RC2_VegetablesRaw",
            "PlantFoodRaw",
            "MeatRaw",
            "RC2_FoodProcessed"
        };

        public static IEnumerable<ThingCategoryDef> AllCats()
        {
            return hotMealCats.Concat(coldMealCats).Concat(hotDrinkCats).Concat(coldDrinkCats).Concat(RTMealCats).Concat(nonperishCats).Concat(rawTastyCats);
        }

        public static List<ThingCategoryDef> AllParents()
        {
            return new List<ThingCategoryDef> { Base.DefOf.DFoodHotMeals, Base.DefOf.DFoodColdMeals, Base.DefOf.DFoodHotDrinks, 
                Base.DefOf.DFoodColdDrinks, Base.DefOf.DFoodRTMeals, Base.DefOf.DFoodNonperishable, Base.DefOf.DFoodRawTasty };
        }

        public static ThingCategoryDef GenerateCategory(ThingCategoryDef tc, string newDef, ThingCategoryDef parent)
        {
            ThingCategoryDef newTCD = new ThingCategoryDef();
            newTCD.defName = newDef;
            newTCD.label = tc.label;
            newTCD.parent = parent;
            newTCD.resourceReadoutRoot = false;
            newTCD.iconPath = tc.iconPath;
            parent.childCategories.Add(newTCD);
            return newTCD;
        }

        public static void HijackCategory(ThingDef def, ThingCategoryDef tc, List<ThingCategoryDef> newCats, ThingCategoryDef parent, string postfix)
        {
            if (tc == null || tc == ThingCategoryDefOf.Root)
                return;
            if (excludedCatDefs.Contains(tc.defName))
                return;
            if (tc.label.ToLower().Contains("(unfert.)") || tc.label.ToLower().Contains("(fert.)"))
                return;
            
            if (tc == ThingCategoryDefOf.Foods)
            {
                HijackDef(def, tc, parent);
                return;
            }
            if (tc.parent == ThingCategoryDefOf.Root)
                return;
            if (tc.parent != ThingCategoryDefOf.Foods)
            {
                HijackCategory(null, tc.parent, newCats, parent, postfix);
            }
            string newDef = tc.defName + postfix;
            ThingCategoryDef found = newCats.Find(x => x.defName == newDef);
            if (found == null)
            {
                found = GenerateCategory(tc, newDef, parent);
                newCats.Add(found);
            }
            if (def != null)
            {
                HijackDef(def, tc, found);
            }
        }

        public static void HijackDef(ThingDef def, ThingCategoryDef tc, ThingCategoryDef newCat = null)
        {
            def.thingCategories.Remove(tc);
            tc.childThingDefs.Remove(def);
            if (newCat != null)
            {
                def.thingCategories.Add(newCat);
                newCat.childThingDefs.Add(def);
            }
        }

        public static void HijackCategories(ThingDef def, List<ThingCategoryDef> newCats, ThingCategoryDef parent, string postfix)
        {
            if (def.thingCategories == null)
                return;
            if (def.label.ToLower().Contains("corpse") || def.defName.ToLower().Contains("corpse"))
                return;
            List<ThingCategoryDef> oldParents = new List<ThingCategoryDef>(def.thingCategories);
            foreach (ThingCategoryDef tc in oldParents)
            {
                if (Base.VGPRunning && (def.defName == "MealSimple" || def.defName == "MealFine" || def.defName == "MealLavish") && tc.defName == "FoodMeals")
                {
                    HijackDef(def, tc);
                    continue;
                }
                HijackCategory(def, tc, newCats, parent, postfix);
            }
        }


        public static void AddHotDrink(ThingDef def)
        {
            def.comps.Add(new CompProperties_DFoodTemperature
            {
                initialTemp = 60,
                likesHeat = true,
                isDrink = true,
                okFrozen = false,
                mealType = MealTempTypes.HotDrink,
                tempLevels = new DTemperatureLevels()
            });
            if (def.tickerType == TickerType.Never)
            {
                def.tickerType = TickerType.Rare;
            }
            HijackCategories(def, hotDrinkCats, Base.DefOf.DFoodHotDrinks, "_hd");
        }

        public static void AddColdDrink(ThingDef def)
        {
            def.comps.Add(new CompProperties_DFoodTemperature
            {
                initialTemp = 22,
                likesHeat = false,
                isDrink = true,
                okFrozen = false,
                diffusionTime = 7500, // 3 hours, more leeway
                mealType = MealTempTypes.ColdDrink,
                tempLevels = new DTemperatureLevels(good: 10f, ok: 23f, bad: 30f, reallyBad: 65f)
            });
            if (def.tickerType == TickerType.Never)
            {
                def.tickerType = TickerType.Rare;
            }
            HijackCategories(def, coldDrinkCats, Base.DefOf.DFoodColdDrinks, "_cd");
        }

        public static void AddColdMeal(ThingDef def)
        {
            def.comps.Add(new CompProperties_DFoodTemperature
            {
                initialTemp = -10,
                likesHeat = false,
                isDrink = false,
                okFrozen = true,
                diffusionTime = 7500,
                mealType = MealTempTypes.ColdMeal,
                tempLevels = new DTemperatureLevels(good: 0f, ok: 5f, bad: 15f, reallyBad: 65f)
            });
            if (def.tickerType == TickerType.Never)
            {
                def.tickerType = TickerType.Rare;
            }

            HijackCategories(def, coldMealCats, Base.DefOf.DFoodColdMeals, "_cm");
        }

        public static void AddHotMeal(ThingDef def)
        {
            def.comps.Add(new CompProperties_DFoodTemperature
            {
                initialTemp = 55f,
                likesHeat = true,
                isDrink = false,
                okFrozen = false,
                mealType = MealTempTypes.HotMeal,
                tempLevels = new DTemperatureLevels()
            });
            if (def.tickerType == TickerType.Never)
            {
                def.tickerType = TickerType.Rare;
            }
            HijackCategories(def, hotMealCats, Base.DefOf.DFoodHotMeals, "_hm");
        }

        public static void AddRoomTemperatureMeal(ThingDef def)
        {
            def.comps.Add(new CompProperties_DFoodTemperature
            {
                initialTemp = 22,
                likesHeat = true,
                isDrink = false,
                okFrozen = false,
                roomTemperature = true,
                mealType = MealTempTypes.RoomTempMeal,
                tempLevels = new DTemperatureLevels(good: 10f, ok: 30f)
            });
            if (def.tickerType == TickerType.Never)
            {
                def.tickerType = TickerType.Rare;
            }
            HijackCategories(def, RTMealCats, Base.DefOf.DFoodRTMeals, "_rtm");
        }

        public static void AddNonperishableMeal(ThingDef def)
        {
            def.comps.Add(new CompProperties_DNoTemp("Nonperishable"));
            HijackCategories(def, nonperishCats, Base.DefOf.DFoodNonperishable, "np");
        }

        public static void AddRawTastyMeal(ThingDef def)
        {
            def.comps.Add(new CompProperties_DNoTemp("Edible raw"));
            HijackCategories(def, rawTastyCats, Base.DefOf.DFoodRawTasty, "rt");
        }

        public static void AddRawResource(ThingDef def)
        {
            def.comps.Add(new CompProperties_DFoodTemperature
            {
                displayName = "Raw resource",
                initialTemp = 22,
                likesHeat = true,
                isDrink = false,
                okFrozen = false,
                mealType = MealTempTypes.RawResource,
                noHeat = true,
                tempLevels = new DTemperatureLevels(good: 1f, ok: -9999f, bad: -9999f, reallyBad: -9999f, frozen: 0f) // bad if frozen, nothing otherwise
            });
            if (def.tickerType == TickerType.Never)
            {
                def.tickerType = TickerType.Rare;
            }
           /* if(def.thingCategories != null && def.thingCategories.Contains(ThingCategoryDefOf.Foods))
            {
                ThingCategoryDef FoodRaw = DefDatabase<ThingCategoryDef>.GetNamedSilentFail("FoodRaw");
                if (FoodRaw != null)
                    HijackDef(def, ThingCategoryDefOf.Foods, FoodRaw);
            }*/

        }
    }
}
