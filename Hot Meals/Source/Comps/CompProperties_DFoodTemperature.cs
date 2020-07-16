using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using DThermodynamicsCore.Comps;

namespace DHotMeals.Comps
{
    public class CompProperties_DFoodTemperature : CompProperties_DTemperatureIngestible
    {
        public CompProperties_DFoodTemperature()
        {
            this.compClass = typeof(CompDFoodTemperature);
        }
        public bool noHeat = false;
        public string displayName = null;
        public MealTempTypes mealType = MealTempTypes.None;
    }
}
