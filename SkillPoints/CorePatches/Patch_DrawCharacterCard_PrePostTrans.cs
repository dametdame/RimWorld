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
using System.Reflection.Emit;

namespace DSkillPoints.CorePatches
{
	[HarmonyPatch(typeof(CharacterCardUtility))]
	[HarmonyPatch("DrawCharacterCard")]
	class Patch_DrawCharacterCard_PrePostTrans
    {
		public const float margin = 5f;
		public const float positionWidth = 258f; // constant for "position" width in base code, used to define width of group for DrawSkillsOf
		
		public static bool Prefix(Pawn pawn, ref Rect rect)
        {
			rect.width += Base.panelAddedWidth;
			return true;
		}
		
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
			var codes = new List<CodeInstruction>(instructions);
			for (int i = 0; i < codes.Count; i++)
            {
				CodeInstruction instruction = codes[i];
				if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float floatop && floatop == positionWidth)
                {
					codes[i] = new CodeInstruction(OpCodes.Ldc_R4, positionWidth + Base.panelAddedWidth);
					break;
                }
            }
			return codes.AsEnumerable();
		}

		private static void Postfix(Rect rect, Pawn pawn, Action randomizeCallback)
		{
			CompSkillPoints spcomp = pawn.TryGetComp<CompSkillPoints>();
			if (spcomp == null)
				return;
			float width = Base.panelAddedWidth - margin;
			float x = CharacterCardUtility.PawnCardSize(pawn).x;
			if (randomizeCallback != null) // this is how the game checks if we're in the character creation screen (shrug)
				x += 175f;
			Rect extraRect = new Rect(x + margin, rect.y, width, 100f);
			ITab_Skills.DrawRankInfo(extraRect, pawn);
		}
	}
}
