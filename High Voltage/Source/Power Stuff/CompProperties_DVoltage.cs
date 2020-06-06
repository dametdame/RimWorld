using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DHighVoltage
{
    public class CompProperties_DVoltage : CompProperties
    {
            public CompProperties_DVoltage()
            {
                this.compClass = typeof(CompDVoltage);
            }

        public float capacityAmp = 1f;
        public float outputAmp = 1f;
            
    }
}
