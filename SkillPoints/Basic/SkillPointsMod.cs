using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace DSkillPoints.Basic
{
    class SkillPointsMod : Mod
    {

        SkillPointsSettings settings;

        public SkillPointsMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<SkillPointsSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            SkillPointsSettings.DrawSettings(inRect);
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Skill Points";
        }


        public override void WriteSettings()
        {
            SkillPointsSettings.WriteAll();
            base.WriteSettings();
        }
    }
}
