using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;


namespace DArcaneTechnology
{
    class CompProperties_DArcane : CompProperties
    {

        public CompProperties_DArcane(ResearchProjectDef rpd)
        {
            this.compClass = typeof(CompDArcane);
            this.project = rpd;
        }

        public ResearchProjectDef project = null;
    }
}
