using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;


namespace DHighVoltage
{
    class HighVoltageMod : Mod
    {
        HighVoltageSettings settings;

        public HighVoltageMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<HighVoltageSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            HighVoltageSettings.DrawSettings(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "High Voltage";
        }


        public override void WriteSettings() // called when settings window closes
        {
            HighVoltageSettings.WriteAll();
            base.WriteSettings();
        }
    }
}
