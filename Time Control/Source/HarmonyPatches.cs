using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using System.Linq;
using System.Reflection.Emit;
using RimWorld;
using DTimeControl.Core_Patches;

namespace DTimeControl
{
    class HarmonyPatches : Mod
    {

		public HarmonyPatches(ModContentPack content) : base(content)
		{
			var harmony = new Harmony("io.github.dametri.timecontrol");
			var assembly = Assembly.GetExecutingAssembly();
			harmony.PatchAll(assembly);
		}

	}
}
