using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace DRimEditor.DetailView
{
	// Token: 0x0200000D RID: 13
	public class DetailSectionHelper
	{
        public static void DrawText(ref Vector3 cur, float width, string text)
        {
            float height = Text.CalcHeight(text, width);
            Rect rect = new Rect(cur.x, cur.y, width, height);
            Widgets.Label(rect, text);
            cur.y += height - 6f; // offset to make lineheights fit better
        }

        public static List<FieldDesc> BuildFieldDescList(object obj, FieldInfo [] fields, FieldDesc parentDesc)
		{
            List<FieldDesc> ret = new List<FieldDesc>();
            foreach (FieldInfo field in fields)
            {
                ret.Add(new FieldDesc(obj, field, parentDesc));
            }
            return ret;
		}
	}
}
