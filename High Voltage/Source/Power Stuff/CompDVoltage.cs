using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DHighVoltage
{
    public class CompDVoltage : ThingComp
    {
        public virtual CompProperties_DVoltage PropsVoltage
        {
            get
            {
                return this.props as CompProperties_DVoltage;
            }
        }

        public override void PostPostMake()
        {
            Log.Message("make conduit");
            capacityMult = HighVoltageBase.GetCapacityMult(this.parent.Stuff);
            outputMult = HighVoltageBase.GetOutputMult(this.parent.Stuff);
            base.PostPostMake();
        }

        public float capacityMult;
        public float outputMult;

    }
}
