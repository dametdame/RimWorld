using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using System.Linq;
using System.Reflection.Emit;
using RimWorld;
using DRimEditor.DetailView;
using System.Collections.Generic;

namespace DRimEditor
{
    class HarmonyPatches : Verse.Mod
	{
		public HarmonyPatches(ModContentPack content) : base(content)
		{
			var harmony = new Harmony("io.github.dametri.rimeditor");
			var assembly = Assembly.GetExecutingAssembly();
			harmony.PatchAll(assembly);

			LongEventHandler.QueueLongEvent(ProfileManager.Init, "DRimEditor.LoadProfiles", false, null) ;
			LongEventHandler.QueueLongEvent(ProfileManager.ApplyActiveProfile, "DRimEditor.ApplyProfiles", false, null);				
		}

	}
}
