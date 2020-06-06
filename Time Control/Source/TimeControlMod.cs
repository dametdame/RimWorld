using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using RimWorld;
namespace DTimeControl
{
    class TimeControlMod : Mod
    {

        TimeControlSettings settings;

        public TimeControlMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<TimeControlSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            TimeControlSettings.DrawSettings(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Time Control";
        }


        public override void WriteSettings() // called when settings window closes
        {
            TimeControlSettings.WriteAll();
            base.WriteSettings();
        }
    }
}