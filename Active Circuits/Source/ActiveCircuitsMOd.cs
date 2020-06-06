using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;


namespace DActiveCircuits
{
    class ActiveCircuitsMod : Mod
    {
        ActiveCircuitsSettings settings;

        public ActiveCircuitsMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<ActiveCircuitsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ActiveCircuitsSettings.DrawSettings(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Active Circuits";
        }


        public override void WriteSettings() // called when settings window closes
        {
            ActiveCircuitsSettings.WriteAll();
            base.WriteSettings();
        }
    }
}