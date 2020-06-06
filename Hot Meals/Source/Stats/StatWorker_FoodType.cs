
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using DHotMeals.Comps;
using DThermodynamicsCore.Comps;

namespace DHotMeals
{
    class StatWorker_FoodType : StatWorker
    {

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (req.HasThing && req.Thing.def.HasComp(typeof(CompDFoodTemperature)))
                return 0;
            return -1;
        }


        public void BuildColdString(CompDFoodTemperature comp, ref StringBuilder s)
        {
            DTemperatureLevels levels = comp.PropsTemp.tempLevels;
            s.AppendLine();
            s.AppendLine("Ideal temperature: <" + GenText.ToStringTemperature(levels.goodTemp));
            s.AppendLine("OK temperature: " + GenText.ToStringTemperature(levels.goodTemp) + " - " + GenText.ToStringTemperature(levels.okTemp));
            s.AppendLine("Bad temperature: " + GenText.ToStringTemperature(levels.okTemp) + " - " + GenText.ToStringTemperature(levels.badTemp));
            s.AppendLine("Awful temperature: >" + GenText.ToStringTemperature(levels.badTemp));
            if (!comp.PropsTemp.okFrozen)
            {
                s.AppendLine();
                s.AppendLine("Eating this below " + GenText.ToStringTemperature(0) + " will cause significant mood penalties.");
            }
        }

        public void BuildHotString(CompDFoodTemperature comp, ref StringBuilder s)
        {
            DTemperatureLevels levels = comp.PropsTemp.tempLevels;
            s.AppendLine();
            s.AppendLine("Ideal temperature: >" + GenText.ToStringTemperature(levels.goodTemp));
            s.AppendLine("OK temperature: " + GenText.ToStringTemperature(levels.okTemp) + " - " + GenText.ToStringTemperature(levels.goodTemp));
            s.AppendLine("Bad temperature: " + GenText.ToStringTemperature(levels.badTemp) + " - " + GenText.ToStringTemperature(levels.okTemp));
            s.AppendLine("Awful temperature: <" + GenText.ToStringTemperature(levels.badTemp));
            if (!comp.PropsTemp.okFrozen)
            {
                s.AppendLine();
                s.AppendLine("Eating this below " + GenText.ToStringTemperature(0) + " will cause significant mood penalties.");
            }
        }

        public void BuildRTString(CompDFoodTemperature comp, ref StringBuilder s)
        {
            DTemperatureLevels levels = comp.PropsTemp.tempLevels;
            s.AppendLine();
            s.AppendLine("Too hot: >" + GenText.ToStringTemperature(levels.okTemp));
            s.AppendLine("Ideal temperature: " + GenText.ToStringTemperature(levels.goodTemp) + " - " + GenText.ToStringTemperature(levels.okTemp));
            s.AppendLine("Too cold: >" + GenText.ToStringTemperature(levels.goodTemp));
            if (!comp.PropsTemp.okFrozen)
            {
                s.AppendLine();
                s.AppendLine("Eating this below " + GenText.ToStringTemperature(0) + " will cause significant mood penalties.");
            }
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            StringBuilder s = new StringBuilder();
            if (!req.HasThing)
                return "";
            CompDFoodTemperature comp = req.Thing.TryGetComp<CompDFoodTemperature>();
            if (comp != null)
            {
                if (comp.PropsTemp.mealType == MealTempTypes.HotMeal)
                {
                    s.AppendLine("Food meant to be eaten hot");
                    BuildHotString(comp, ref s);
                }
                else if (comp.PropsTemp.mealType == MealTempTypes.ColdMeal)
                {
                    s.AppendLine("Food meant to be eaten cold");
                    BuildColdString(comp, ref s);
                }
                else if (comp.PropsTemp.mealType == MealTempTypes.HotDrink)
                {
                    s.AppendLine("A drink that's meant to be served hot");
                    BuildHotString(comp, ref s);
                }
                else if (comp.PropsTemp.mealType == MealTempTypes.ColdDrink)
                {
                    s.AppendLine("A drink best served chilled, but not frozen");
                    BuildColdString(comp, ref s);
                }
                else if (comp.PropsTemp.mealType == MealTempTypes.RoomTempMeal)
                {
                    s.AppendLine("Food best served at room temperature - neither hot nor cold.");
                    BuildRTString(comp, ref s);
                }  
                else if (comp.PropsTemp.mealType == MealTempTypes.RawResource)
                {
                    s.AppendLine("Resources meant to be cooked into something more edible.\nWill never make your colonists happy to eat, but will make them unhappy if they eat it frozen.");
                }
            }
            return s.ToString();
        }

        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine();
            s.AppendLine();
            s.AppendLine();
            s.AppendLine();
            s.AppendLine();
            s.AppendLine();
            s.AppendLine();
            s.AppendLine();
            return s.ToString();
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            if (!optionalReq.HasThing)
                return "None";
            CompDFoodTemperature comp = optionalReq.Thing.TryGetComp<CompDFoodTemperature>();
            if (comp != null)
            {
                return comp.GetFoodType();
            }
            return "None";
        }

    }
}
