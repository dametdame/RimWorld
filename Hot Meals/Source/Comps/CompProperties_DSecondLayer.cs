using System;
using Verse;

namespace DHotMeals.Comps
{
    public class CompProperties_DSecondLayer : CompProperties
	{
		public float Altitude
		{
			get
			{
				return this.altitudeLayer.AltitudeFor();
			}
		}

		public CompProperties_DSecondLayer()
		{
			this.compClass = typeof(CompDSecondLayer);
		}

		public GraphicData graphicData;

		public AltitudeLayer altitudeLayer = AltitudeLayer.MoteOverhead;
	}
}
