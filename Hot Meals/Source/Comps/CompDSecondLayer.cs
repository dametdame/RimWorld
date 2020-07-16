using System;
using RimWorld;
using Verse;

namespace DHotMeals.Comps
{
    public class CompDSecondLayer : ThingComp
    {
		public CompProperties_DSecondLayer Props
		{
			get
			{
				return (CompProperties_DSecondLayer)this.props;
			}
		}

		public virtual Graphic Graphic
		{
			get
			{
				if (this.graphicInt == null)
				{
					if (this.Props.graphicData == null)
					{
						ThingDef def = this.parent.def;
						Log.ErrorOnce(((def != null) ? def.ToString() : null) + " has no DSecondLayer graphicData but we are trying to access it.", 764533, false);
						return BaseContent.BadGraphic;
					}
					this.graphicInt = this.Props.graphicData.GraphicColoredFor(this.parent);
				}
				return this.graphicInt;
			}
		}

		public override void PostDraw()
		{
			base.PostDraw();
			this.Graphic.Draw(GenThing.TrueCenter(this.parent.Position, this.parent.Rotation, this.parent.def.size, this.Props.Altitude), this.parent.Rotation, this.parent, 0f);
		}

		private Graphic graphicInt;
	}
}
