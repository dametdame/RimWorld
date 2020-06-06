using System;
using UnityEngine;
using Verse;

namespace DRimEditor.DetailView
{

	public static class Texture2DExtensions
	{
		public static Texture2D Crop(this Texture2D tex)
		{
			return tex;
		}

		public static void DrawFittedIn(this Texture2D tex, Rect rect)
		{
			float rectProportion = rect.width / rect.height;
			float texProportion = (float)tex.width / (float)tex.height;
			bool flag = texProportion > rectProportion;
			if (flag)
			{
				Rect wider = new Rect(rect.xMin, 0f, rect.width, rect.width / texProportion).CenteredOnYIn(rect).CenteredOnXIn(rect);
				GUI.DrawTexture(wider, tex);
			}
			else
			{
				bool flag2 = texProportion < rectProportion;
				if (flag2)
				{
					Rect taller = new Rect(0f, rect.yMin, rect.height * texProportion, rect.height).CenteredOnXIn(rect).CenteredOnXIn(rect);
					GUI.DrawTexture(taller, tex);
				}
				else
				{
					GUI.DrawTexture(rect, tex);
				}
			}
		}
	}
}
