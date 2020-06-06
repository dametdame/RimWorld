using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
namespace DArcaneTechnology
{
    class CompDArcane : ThingComp
    {
        public virtual CompProperties_DArcane PropsArcane
        {
            get
            {
                return (CompProperties_DArcane)this.props;
            }
        }

        public override string CompInspectStringExtra()
        {
            if (Base.IsResearchLocked(this.parent.def))
                return "Unknown technology (" + PropsArcane.project.LabelCap + ")";
            else
                return base.CompInspectStringExtra();
        }

    }
}
