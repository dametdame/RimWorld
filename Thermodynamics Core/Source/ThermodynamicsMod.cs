using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;

namespace DThermodynamicsCore
{
    class ThermodynamicsMod : Mod
    {
        ThermodynamicsSettings settings;

        public ThermodynamicsMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<ThermodynamicsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ThermodynamicsSettings.DrawSettings(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Thermodynamics - Core";
        }


        public override void WriteSettings() // called when settings window closes
        {
            ThermodynamicsSettings.WriteAll();
            base.WriteSettings();
        }
    }
}