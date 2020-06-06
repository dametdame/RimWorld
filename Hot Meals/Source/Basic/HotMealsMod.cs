
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;

namespace DHotMeals
{
    class HotMealsMod : Mod
    {
        HotMealsSettings settings;

        public HotMealsMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<HotMealsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            HotMealsSettings.DrawSettings(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Thermodynamics - Hot Meals";
        }


        public override void WriteSettings() // called when settings window closes
        {
            HotMealsSettings.WriteAll();
            base.WriteSettings();
        }
    }
}