using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace DRimEditor.DetailView
{
	// Token: 0x0200000A RID: 10
	public static class DefExtensions
	{
		// Token: 0x0600003E RID: 62 RVA: 0x00003664 File Offset: 0x00001864
		public static string LabelStyled(this Def def)
		{
			bool flag = def.label.NullOrEmpty();
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = !def.description.NullOrEmpty();
				if (flag2)
				{
					result = "<i>" + def.LabelCap + "</i>";
				}
				else
				{
					result = def.LabelCap;
				}
			}
			return result;
		}

	
		// Token: 0x0600003F RID: 63 RVA: 0x000036CE File Offset: 0x000018CE
		public static void DrawColouredIcon(this Def def, Rect canvas)
		{
			GUI.color = def.IconColor();
			GUI.DrawTexture(canvas, def.IconTexture(), ScaleMode.ScaleToFit);
			GUI.color = Color.white;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x000036F8 File Offset: 0x000018F8
		public static Color IconColor(this Def def)
		{
			bool flag = DefExtensions._cachedIconColors.ContainsKey(def);
			Color result;
			if (flag)
			{
				result = DefExtensions._cachedIconColors[def];
			}
			else
			{
				BuildableDef bdef = def as BuildableDef;
				ThingDef tdef = def as ThingDef;
				PawnKindDef pdef = def as PawnKindDef;
				RecipeDef rdef = def as RecipeDef;
				bool flag2 = rdef != null;
				if (flag2)
				{
					bool flag3 = !rdef.products.NullOrEmpty<ThingDefCountClass>();
					if (flag3)
					{
						DefExtensions._cachedIconColors.Add(def, rdef.products.First<ThingDefCountClass>().thingDef.IconColor());
						return DefExtensions._cachedIconColors[def];
					}
				}
				bool flag4 = pdef != null;
				if (flag4)
				{
					DefExtensions._cachedIconColors.Add(def, pdef.lifeStages.Last<PawnKindLifeStage>().bodyGraphicData.color);
					result = DefExtensions._cachedIconColors[def];
				}
				else
				{
					bool flag5 = bdef == null;
					if (flag5)
					{
						DefExtensions._cachedIconColors.Add(def, Color.white);
						result = DefExtensions._cachedIconColors[def];
					}
					else
					{
						bool flag6 = tdef != null && tdef.entityDefToBuild != null;
						if (flag6)
						{
							DefExtensions._cachedIconColors.Add(def, tdef.entityDefToBuild.IconColor());
							result = DefExtensions._cachedIconColors[def];
						}
						else
						{
							bool flag7 = bdef.graphic != null;
							if (flag7)
							{
								DefExtensions._cachedIconColors.Add(def, bdef.graphic.color);
								result = DefExtensions._cachedIconColors[def];
							}
							else
							{
								bool flag8 = tdef != null && tdef.MadeFromStuff;
								if (flag8)
								{
									ThingDef stuff = GenStuff.DefaultStuffFor(tdef);
									DefExtensions._cachedIconColors.Add(def, stuff.stuffProps.color);
									result = DefExtensions._cachedIconColors[def];
								}
								else
								{
									DefExtensions._cachedIconColors.Add(def, Color.white);
									result = DefExtensions._cachedIconColors[def];
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x000038E0 File Offset: 0x00001AE0
		public static Texture2D IconTexture(this Def def)
		{
			bool flag = DefExtensions._cachedDefIcons.ContainsKey(def);
			Texture2D result;
			if (flag)
			{
				result = DefExtensions._cachedDefIcons[def];
			}
			else
			{
				BuildableDef bdef = def as BuildableDef;
				ThingDef tdef = def as ThingDef;
				PawnKindDef pdef = def as PawnKindDef;
				RecipeDef rdef = def as RecipeDef;
				bool flag2 = rdef != null && !rdef.products.NullOrEmpty<ThingDefCountClass>();
				if (flag2)
				{
					DefExtensions._cachedDefIcons.Add(def, rdef.products.First<ThingDefCountClass>().thingDef.IconTexture());
					result = DefExtensions._cachedDefIcons[def];
				}
				else
				{
					bool flag3 = pdef != null;
					if (flag3)
					{
						try
						{
							DefExtensions._cachedDefIcons.Add(def, (pdef.lifeStages.Last<PawnKindLifeStage>().bodyGraphicData.Graphic.MatSouth.mainTexture as Texture2D).Crop());
							return DefExtensions._cachedDefIcons[def];
						}
						catch
						{
						}
					}
					bool flag4 = bdef == null;
					if (flag4)
					{
						DefExtensions._cachedDefIcons.Add(def, null);
						result = null;
					}
					else
					{
						bool flag5 = tdef != null;
						if (flag5)
						{
							bool flag6 = tdef.entityDefToBuild != null;
							if (flag6)
							{
								DefExtensions._cachedDefIcons.Add(def, tdef.entityDefToBuild.IconTexture().Crop());
								return DefExtensions._cachedDefIcons[def];
							}
							bool isCorpse = tdef.IsCorpse;
							if (isCorpse)
							{
								return null;
							}
						}
						DefExtensions._cachedDefIcons.Add(def, bdef.uiIcon.Crop());
						result = bdef.uiIcon.Crop();
					}
				}
			}
			return result;
		}
		
		// Token: 0x06000042 RID: 66 RVA: 0x00003A84 File Offset: 0x00001C84
		public static float StyledLabelAndIconSize(this Def def)
		{
			bool WW = Text.WordWrap;
			Text.WordWrap = false;
			float width = Text.CalcSize(def.LabelStyled()).x + (float)((def.IconTexture() == null) ? 0 : 20);
			Text.WordWrap = WW;
			return width;
		}

		public static ItemDetailDef GetDef(this Def def)
		{
			
			bool flag = DefExtensions._cachedDefHelpDefLinks.ContainsKey(def);
			ItemDetailDef result;
			if (flag)
			{
				result = DefExtensions._cachedDefHelpDefLinks[def];
			}
			else
			{
				DefExtensions._cachedDefHelpDefLinks.Add(def, DefDatabase<ItemDetailDef>.AllDefsListForReading.FirstOrDefault((ItemDetailDef hd) => hd.keyDef == def));
				result = DefExtensions._cachedDefHelpDefLinks[def];
			}
			return result;
		}

		// Token: 0x0400002A RID: 42
		private static Dictionary<Def, Texture2D> _cachedDefIcons = new Dictionary<Def, Texture2D>();

		// Token: 0x0400002B RID: 43
		private static Dictionary<Def, Color> _cachedIconColors = new Dictionary<Def, Color>();

		private static Dictionary<Def, ItemDetailDef> _cachedDefHelpDefLinks = new Dictionary<Def, ItemDetailDef>();
	}
}
