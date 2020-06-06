using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using System.Reflection.Emit;
using System.Reflection;
using UnityEngine.UI;
using Verse.AI;

namespace DTimeControl.Core_Patches
{
    public static class GenericTickReplacer
    {

        public static IEnumerable<CodeInstruction> ReplaceTicks(IEnumerable<CodeInstruction> instructions, string name)
        {
			var codes = new List<CodeInstruction>(instructions);
			List<int> indices = new List<int>();
			for (int i = 0; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Callvirt)
				{
					var type = codes[i].operand as System.Reflection.MethodInfo;
					if (type != null)
					{
						if (type.Name == "get_TicksGame")
						{
							indices.Add(i);
						}
					}
				}
			}
			if (indices.Count > 0)
			{
				foreach (int index in indices)
				{
					var mi = AccessTools.Field(typeof(TickUtility), nameof(TickUtility.adjustedTicksGameInt));
					var modCall = new CodeInstruction(OpCodes.Ldfld, mi);
					codes[index - 1].opcode = OpCodes.Ldnull;
					codes[index] = modCall;
				}
			}
			else
			{
				Log.Warning("Failed to find any calls to TicksGame in " + name +" transpiler, nothing changed");
			}
			return codes;
		}
    }
}
