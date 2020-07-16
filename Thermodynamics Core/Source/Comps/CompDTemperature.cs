using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using UnityEngine;
using Verse.Noise;

namespace DThermodynamicsCore.Comps
{
    public class CompDTemperature : ThingComp
    {

		public virtual CompProperties_DTemperature PropsTemp
		{
			get
			{
				return (CompProperties_DTemperature)this.props;
			}
		}

		public virtual DTemperatureLevels Levels
		{
			get
			{
				return this.PropsTemp.tempLevels;
			}
		}

		public virtual double AmbientTemperature
		{
			get
			{
				if (this.parent.Spawned)
				{
					return GenTemperature.GetTemperatureForCell(this.parent.Position, this.parent.Map);
				}
				if (this.parent.ParentHolder != null)
				{
					for (IThingHolder parentHolder = this.parent.ParentHolder; parentHolder != null; parentHolder = parentHolder.ParentHolder)
					{

						float result;
						if (ThingOwnerUtility.TryGetFixedTemperature(parentHolder, this.parent, out result))
						{
							return result;
						}
					}
				}
				if (this.parent.SpawnedOrAnyParentSpawned)
				{
					return GenTemperature.GetTemperatureForCell(this.parent.PositionHeld, this.parent.MapHeld);
				}
				if (this.parent.Tile >= 0)
				{
					return GenTemperature.GetTemperatureAtTile(this.parent.Tile);
				}
				return 21f;
			}
		}

		public virtual float GetCurTemp()
        {
			return (float)this.curTemp;
        }

		public virtual string GetState(double temp)
		{
			return "";
		}

		public virtual bool PawnIsCarrying()
		{
			if (this.parent.ParentHolder != null)
			{
				for (IThingHolder parentHolder = this.parent.ParentHolder; parentHolder != null; parentHolder = parentHolder.ParentHolder)
				{
					if (parentHolder is Pawn_InventoryTracker)
					{
						return true;
					}
				}
			}
			return false;
		}


		public virtual void Diffuse(double diffuseTo, int interval)
		{
			double diffuseTime = PropsTemp.diffusionTime;
			if (ThermodynamicsSettings.slowDiffuseWhileCarried && PawnIsCarrying())
			{
				diffuseTime *= 10;
			}
			double minStepScaled = minStep * interval;
			double shift = diffuseTo - curTemp;
			double changeMag = Math.Abs(interval * shift / diffuseTime);
			double step = (Math.Abs(shift) < minStepScaled || changeMag > CompDTemperature.minStep) ? changeMag : minStepScaled;
			curTemp += Math.Sign(shift) * step * ThermodynamicsSettings.diffusionModifier;
		}


		public virtual void tempTick(int numTicks)
		{
			this.Diffuse(this.AmbientTemperature, numTicks);
		}

		public override void CompTick()
		{
			tickCount++;
			if (tickCount >= PropsTemp.tickRate)
			{
				tickCount = 0;
				this.tempTick(PropsTemp.tickRate);
			}
		}

		public override void CompTickRare()
		{
			this.tempTick(250);
		}


		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
		}

		public override string CompInspectStringExtra()
		{
			return GenText.ToStringTemperature((float)curTemp);
		}

		public override string GetDescriptionPart()
		{
			return base.GetDescriptionPart();
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<double>(ref this.curTemp, "curTemp", PropsTemp.initialTemp, false);	
		}

		public override void PostPostMake()
		{
			base.PostPostMake();
			this.curTemp = PropsTemp.initialTemp;
		}

		public override void PostSplitOff(Thing piece)
		{
			CompDTemperature newComp = piece.TryGetComp<CompDTemperature>();
			if (newComp != null)
			{
				newComp.curTemp = curTemp;
			}
			base.PostSplitOff(piece);
		}

		public override void PreAbsorbStack(Thing otherStack, int count)
		{
			CompDTemperature newComp = otherStack.TryGetComp<CompDTemperature>();
			if (newComp != null)
			{
				this.curTemp = (this.curTemp * this.parent.stackCount + newComp.curTemp * count) / (float)(this.parent.stackCount + count);
			}
			base.PreAbsorbStack(otherStack, count);
		}

		private int tickCount = 0;

		public double curTemp;

		public const double minStep = 10.0 / 2500.0; // 3 degrees celsius = 5.4 degrees fahrenheit per hour at minimum

	}
}
