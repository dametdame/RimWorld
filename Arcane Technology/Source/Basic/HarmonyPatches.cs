using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using System.Linq;
using System.Reflection.Emit;
using RimWorld;
using DArcaneTechnology.CorePatches;

namespace DArcaneTechnology
{
	public class HarmonyPatches : Verse.Mod
	{
		public HarmonyPatches(ModContentPack content) : base(content)
		{
			var harmony = new Harmony("io.github.dametri.arcanetechnology");
			var assembly = Assembly.GetExecutingAssembly();
			harmony.PatchAll(assembly);

			var original = AccessTools.Method(typeof(EquipmentUtility), "CanEquip", new[] { typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType() });
			var postfix = AccessTools.Method(typeof(Patch_CanEquip_Postfix), "Postfix", new[] { typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool).MakeByRefType() });
			if (original != null && postfix != null)
				harmony.Patch(original, postfix: new HarmonyMethod(postfix));
		}

	}
}
