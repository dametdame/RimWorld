using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;


namespace DThermodynamicsCore.Comps
{
    public class CompProperties_DNoTemp : CompProperties
    {
        public CompProperties_DNoTemp(string inspect)
        {
            this.compClass = typeof(CompDNoTemp);
            this.inspectString = inspect;
        }

        public string inspectString = "";
    }
}
