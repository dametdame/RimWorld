using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.AI;

namespace DThermodynamicsCore.Comps
{
    public class CompDTemperatureIngestible : CompDTemperature
    {

		public new virtual CompProperties_DTemperatureIngestible PropsTemp
		{
			get
			{
				return (CompProperties_DTemperatureIngestible)this.props;
			}
		}

		public override string GetState(double temp)
		{
			return base.GetState(temp);
		}

		public virtual ThoughtDef GetIngestMemory()
		{
			return null;
		}

		public virtual void MakeIngestMemory(ThoughtDef memory, Pawn ingester)
		{
			return;
		}

		public override void PostIngested(Pawn ingester)
		{
			CompDTemperature temp = ingester.TryGetComp<CompDTemperature>();
			if (temp != null)
			{
				temp.curTemp += (this.curTemp - temp.curTemp) / 5.0f;
			}
			if (ingester.needs.mood != null)
			{
				ThoughtDef memory = GetIngestMemory();
				if (memory != null)
				{
					MakeIngestMemory(memory, ingester);
					
				}
			}
			base.PostIngested(ingester);

		}

		public override string CompInspectStringExtra()
		{
			string s = GetState(curTemp);
			if (s != "")
				return base.CompInspectStringExtra() + " (" + s + ")";
			return base.CompInspectStringExtra();
		}


	}


}
