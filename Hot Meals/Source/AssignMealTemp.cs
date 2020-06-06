using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using System.Xml;

namespace Verse
{
    public class AssignMealTemp : PatchOperation
    {

        protected override bool ApplyWorker(XmlDocument xml)
        {
            foreach(Meal m in Meals)
            {
                DHotMeals.MealAssigner.fixedTypes.SetOrAdd(m.name, m.type);
            }
            return true;
        }

        public List<Meal> Meals;
    }

    public class Meal
    {
        public string name;
        public DHotMeals.MealTempTypes type;

        public Meal() { }
    }
}
