using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimWorld;


namespace DArcaneTechnology
{
    class ArcaneTechnologyMod : Mod
    {
        ArcaneTechnologySettings settings;

        public ArcaneTechnologyMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<ArcaneTechnologySettings>();

            LongEventHandler.QueueLongEvent(Base.Initialize, "DArcaneTech.BuildingDatabase", false, null);
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            ArcaneTechnologySettings.DrawSettings(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Arcane Technology";
        }


        public override void WriteSettings() // called when settings window closes
        {
            ArcaneTechnologySettings.WriteAll();
            base.WriteSettings();
        }
    }
}