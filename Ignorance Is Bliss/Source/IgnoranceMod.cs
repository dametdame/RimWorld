using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;

namespace DIgnoranceIsBliss
{
    class IgnoranceMod : Mod
    {
        IgnoranceSettings settings;

        public IgnoranceMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<IgnoranceSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            IgnoranceSettings.DrawSettings(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Ignorance Is Bliss";
        }


        public override void WriteSettings() // called when settings window closes
        {
            IgnoranceSettings.WriteAll();
            base.WriteSettings();
        }
    }
}