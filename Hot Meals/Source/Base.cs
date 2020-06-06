using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;
using DHotMeals.Comps;
using Verse.AI;
using DThermodynamicsCore.Comps;

namespace DHotMeals
{
    [StaticConstructorOnStartup]
    public static class Base
    {

        public static bool VGPRunning = false;
      

        [RimWorld.DefOf]
        public static class DefOf
        {
            // microwave
            public static ThingDef DMicrowave;
            // thing categories
            public static ThingCategoryDef DFoodHotMeals;
            public static ThingCategoryDef DFoodColdMeals;
            public static ThingCategoryDef DFoodHotDrinks;
            public static ThingCategoryDef DFoodColdDrinks;
            public static ThingCategoryDef DFoodRTMeals;
            public static ThingCategoryDef DFoodNonperishable;
            public static ThingCategoryDef DFoodRawTasty;
            // job
            public static JobDef HeatMeal;
            // thoughts
            public static ThoughtDef DAteGoodThing;
            public static ThoughtDef DAteTooHot;
            public static ThoughtDef DAteTooCold;
            public static ThoughtDef DAteMeh;
            
        }

        public static bool IsExplicitlyDefined(ThingDef def)
        {
            string s = MealAssigner.fixedTypes.Keys.ToList().FindLast(x => def.defName.ToLower() == x.ToLower() || def.label.ToLower() == x.ToLower());
            if (s != null)
            {
                MealTempTypes category = MealAssigner.fixedTypes[s];
                if (category == MealTempTypes.None)
                    return true;
                else if (category == MealTempTypes.HotMeal)
                    MealAssigner.AddHotMeal(def);
                else if (category == MealTempTypes.ColdMeal)
                    MealAssigner.AddColdMeal(def);
                else if (category == MealTempTypes.HotDrink)
                    MealAssigner.AddHotDrink(def);
                else if (category == MealTempTypes.ColdDrink)
                    MealAssigner.AddColdDrink(def);
                else if (category == MealTempTypes.RoomTempMeal)
                    MealAssigner.AddRoomTemperatureMeal(def);
                else if (category == MealTempTypes.NonPerishable)
                    MealAssigner.AddNonperishableMeal(def);
                else if (category == MealTempTypes.RawTasty)
                    MealAssigner.AddRawTastyMeal(def);
                else if (category == MealTempTypes.RawResource)
                    MealAssigner.AddRawResource(def);
                return true;
            }
            return false;
        }

        static Base()
        {
            if(LoadedModManager.RunningModsListForReading.Any(x => x.Name == "VGP Vegetable Garden"))
            {
                VGPRunning = true;
            }

            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            { 
                if (MealSorter.IsRace(def))
                {
                    //MealAssigner.addRace(def); // not today
                    continue;
                }
                else if (def.ingestible == null)
                {
                    continue;
                }
                else if (IsExplicitlyDefined(def))
                {
                    continue;
                }
                else if (MealSorter.IsExcluded(def))
                {
                    continue;
                }
                else if (MealSorter.IsRawTasty(def))
                {
                    MealAssigner.AddRawTastyMeal(def);
                }
                else if (MealSorter.IsHotDrink(def))
                {
                    MealAssigner.AddHotDrink(def);
                }
                else if (MealSorter.IsColdDrink(def))
                {
                    MealAssigner.AddColdDrink(def);
                }
                else if (MealSorter.IsRawResource(def))
                {
                    MealAssigner.AddRawResource(def);
                }
                else if (MealSorter.IsNonPerishable(def))
                {
                    MealAssigner.AddNonperishableMeal(def);
                }
                else if (MealSorter.IsColdMeal(def))
                {
                    MealAssigner.AddColdMeal(def);
                }
                else if (MealSorter.IsHotMeal(def))
                {
                    MealAssigner.AddHotMeal(def);
                }
            }

            foreach(ThingCategoryDef tc in MealAssigner.AllCats())
            {
                tc.ResolveReferences();
                tc.PostLoad();
            }

            ResourceCounter.ResetDefs();
            DefDatabase<ThingCategoryDef>.ResolveAllReferences(true, false);
            DefDatabase<RecipeDef>.ResolveAllReferences(true, false);
        }

      

    }
}
