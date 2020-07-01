using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using System.Reflection;
using UnityEngine;
using DSkillPoints.Basic;

namespace DSkillPoints.CorePatches
{
    [HarmonyPatch(typeof(SkillUI))]
    [HarmonyPatch("DrawSkillsOf")]
    class Patch_DrawSkillsOf_Pre
    {
		public const float margin = 5f;
		public const float skillHeight = 24f; // constant in base game's code under SkillUI
		public const float skillRowHeight = 27f; // constant ''
		public const float buttonHeight = 18f;
		public const float skillWidth = 230f; // constant ''
		public const float leftRectWidth = 250f; // constant under CharacterCardUtility
		public const float x = leftRectWidth + skillWidth + margin;
		public const float checkHeight = 16f;

		public const float gap = (skillHeight - buttonHeight) / 2f;
		public const float checkGap = (skillHeight - checkHeight) / 2f;

		// temporary, delete later
		public static Dictionary<SkillRecord, bool> autoChecked = new Dictionary<SkillRecord, bool>();

		private static bool Prefix(Pawn p, ref Vector2 offset)
        {
			if (Current.ProgramState != ProgramState.Playing || Current.Game == null || !p.IsColonist)
				return true;

			CompSkillPoints spcomp = p.TryGetComp<CompSkillPoints>();
			if (spcomp == null)
				return true;

			FieldInfo sdiloc = AccessTools.Field(typeof(SkillUI), "skillDefsInListOrderCached");
			List<SkillDef> skillDefsInListOrderCached = sdiloc.GetValue(null) as List<SkillDef>;		

			float y = 0;
			
			bool drawBox = true;
			foreach (SkillDef skill in skillDefsInListOrderCached)
			{
				// highlight rows if set
				if (SkillPointsSettings.highlightAlternate && drawBox)
				{
					Rect box = new Rect(0 + offset.x, y + offset.y, skillWidth + skillHeight, skillHeight);
					Widgets.DrawBoxSolid(box, Base.alternateRowColor);
				}
				drawBox = !drawBox;

				// draw buttons
				SkillRecord rec = p.skills.GetSkill(skill);
				if (!rec.TotallyDisabled && rec.Level < 20)
				{
					// level up (+) button
					Rect levelUpRect = new Rect(offset.x, y + gap + offset.y, buttonHeight, buttonHeight);
					bool clicked = Widgets.ButtonText(levelUpRect, "+");
					if (clicked)
					{
						Base.DoSkillLevelUp(p, rec);
					}
					// autolevel radio button
					//Rect autoLevelRect = new Rect(skillWidth + gap + offset.x, y + gap + offset.y, buttonHeight, buttonHeight);
					if (!autoChecked.ContainsKey(rec))
						autoChecked.Add(rec, false);
					bool newCheck = autoChecked[rec];
					Widgets.Checkbox(new Vector2(skillWidth + checkGap + offset.x, y + checkGap + offset.y), ref newCheck, size: checkHeight);
					if (newCheck)
					{
						List<SkillRecord> keys = autoChecked.Keys.ToList();
						foreach (SkillRecord sr in keys)
						{
							autoChecked[sr] = false;
						}
						autoChecked[rec] = true;
						Text.Font = GameFont.Tiny;
						Rect autoLabel = new Rect(skillWidth + checkGap + offset.x + checkHeight + margin, y + checkGap + offset.y, 150f, Text.LineHeight);
						Widgets.Label(autoLabel, "Auto-level " + skill.LabelCap);
						Text.Font = GameFont.Small;
					}
					else
						autoChecked[rec] = false;
				}
				y += skillRowHeight;
			}
			bool manualCheck = Base.IsOnManual(p);
			Widgets.Checkbox(new Vector2(skillWidth + checkGap + offset.x, y + checkGap + offset.y), ref manualCheck, size: checkHeight, disabled: manualCheck);
			Rect manualLabel = new Rect(skillWidth + checkGap + offset.x + checkHeight + margin, y + checkGap + offset.y, 100f, Text.LineHeight);
			Text.Font = GameFont.Tiny;
			Widgets.Label(manualLabel, "Manual");
			Text.Font = GameFont.Small;
			// shift actual skill drawing to the right
			offset.x += buttonHeight;
			return true;
		}
    }
}
