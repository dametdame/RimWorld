using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using DRimEditor.DetailView;

namespace DRimEditor.DetailView
{
	// Token: 0x02000010 RID: 16
	public struct DefStringTriplet
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00004A16 File Offset: 0x00002C16
		public DefStringTriplet(Def def, string prefix = null, string suffix = null)
		{
			this.Def = def;
			this.Prefix = prefix;
			this.Suffix = suffix;
			this._height = 0f;
			this._heightSet = false;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004A40 File Offset: 0x00002C40
		public override string ToString()
		{
			StringBuilder s = new StringBuilder();
			bool flag = this.Prefix != "";
			if (flag)
			{
				s.Append(this.Prefix + " ");
			}
			s.Append(this.Def.LabelCap);
			bool flag2 = this.Suffix != "";
			if (flag2)
			{
				s.Append(" " + this.Suffix);
			}
			return s.ToString();
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00004AD0 File Offset: 0x00002CD0
		public void Draw(ref Vector3 cur, Vector3 colWidths, DefView window = null)
		{
			bool flag = !this._heightSet;
			if (flag)
			{
				List<float> heights = new List<float>();
				bool flag2 = !this.Prefix.NullOrEmpty();
				if (flag2)
				{
					heights.Add(Text.CalcHeight(this.Prefix, colWidths.x));
				}
				heights.Add(Text.CalcHeight(this.Def.LabelStyled(), colWidths.y));
				bool flag3 = !this.Suffix.NullOrEmpty();
				if (flag3)
				{
					heights.Add(Text.CalcHeight(this.Suffix, colWidths.z));
				}
				this._height = heights.Max();
				this._heightSet = true;
			}
			bool flag4 = !this.Prefix.NullOrEmpty();
			if (flag4)
			{
				Rect prefixRect = new Rect(cur.x, cur.y, colWidths.x, this._height);
				Widgets.Label(prefixRect, this.Prefix);
			}
			bool flag5 = !this.Suffix.NullOrEmpty();
			if (flag5)
			{
				Rect suffixRect = new Rect(cur.x + colWidths.x + colWidths.y + 2f * DetailSection._columnMargin, cur.y, colWidths.z, this._height);
				Widgets.Label(suffixRect, this.Suffix);
			}
			bool flag6 = this.Def.IconTexture() != null;
			Rect labelRect;
			if (flag6)
			{
				Rect iconRect = new Rect(cur.x + colWidths.x + (float)(this.Prefix.NullOrEmpty() ? 0 : 1) * DetailSection._columnMargin, cur.y + 2f, 16f, 16f);
				labelRect = new Rect(cur.x + colWidths.x + 20f + (float)(this.Prefix.NullOrEmpty() ? 0 : 1) * DetailSection._columnMargin, cur.y, colWidths.y - 20f, this._height);
				this.Def.DrawColouredIcon(iconRect);
				Widgets.Label(labelRect, this.Def.LabelStyled());
			}
			else
			{
				labelRect = new Rect(cur.x + colWidths.x + (float)(this.Prefix.NullOrEmpty() ? 0 : 1) * DetailSection._columnMargin, cur.y, colWidths.y, this._height);
				Widgets.Label(labelRect, this.Def.LabelStyled());
			}
			/*HelpDef helpDef = this.Def.GetHelpDef();
			bool flag7 = window != null && helpDef != null;
			if (flag7)
			{
				TooltipHandler.TipRegion(labelRect, this.Def.description + (this.Def.description.NullOrEmpty() ? "" : "\n\n") + ResourceBank.String.JumpToTopic);
				bool flag8 = Widgets.ButtonInvisible(labelRect, true);
				if (flag8)
				{
					bool flag9 = window.Accept(helpDef);
					if (flag9)
					{
						window.JumpTo(helpDef);
					}
					else
					{
						window.SecondaryView(helpDef).JumpTo(helpDef);
					}
				}
			}
			bool flag10 = helpDef == null && !this.Def.description.NullOrEmpty();
			if (flag10)
			{
				TooltipHandler.TipRegion(labelRect, this.Def.description);
			}
			cur.y += this._height - 6f;*/
		}

		// Token: 0x0400003D RID: 61
		public Def Def;

		// Token: 0x0400003E RID: 62
		public string Prefix;

		// Token: 0x0400003F RID: 63
		public string Suffix;

		// Token: 0x04000040 RID: 64
		private float _height;

		// Token: 0x04000041 RID: 65
		private bool _heightSet;
	}
}
