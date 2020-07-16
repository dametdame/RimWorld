using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

// basically the same as CompProperties_TempControl, but they made it annoying to inherit and modify

namespace DThermodynamicsCore.Comps
{
    public class CompProperties_DTempControl : CompProperties
    {
		public CompProperties_DTempControl()
		{
			this.compClass = typeof(CompDTempControl);
		}

		public float defaultTargetTemperature = 21f;
		public float minTargetTemperature = -50f;
		public float maxTargetTemperature = 50f;
	}
}
