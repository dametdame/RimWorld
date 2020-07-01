using System;
using UnityEngine;
using Verse;

namespace DRimEditor.DetailView
{
	public static class ResourceBank
	{
		[StaticConstructorOnStartup]
		public static class Icon
		{
			public static readonly Texture2D HelpMenuArrowUp = ContentFinder<Texture2D>.Get("UI/HelpMenu/ArrowUp", true);
			public static readonly Texture2D HelpMenuArrowDown = ContentFinder<Texture2D>.Get("UI/HelpMenu/ArrowDown", true);
			public static readonly Texture2D HelpMenuArrowRight = ContentFinder<Texture2D>.Get("UI/HelpMenu/ArrowRight", true);
		}

		[StaticConstructorOnStartup]
		public static class String
		{
			public static readonly string RimEditorDefs = "RimEditorDefs".Translate();
		}
	}
}
