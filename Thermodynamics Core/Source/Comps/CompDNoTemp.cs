using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DThermodynamicsCore.Comps
{
    public class CompDNoTemp : ThingComp
    {
        public virtual CompProperties_DNoTemp PropsNoTemp
        {
            get
            {
                return (CompProperties_DNoTemp)this.props;
            }
        }

        public override string CompInspectStringExtra()
        {
            return this.PropsNoTemp.inspectString;
        }
       
    }
}
