using DThermodynamicsCore.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DThermodynamicsCore.Comps
{
    public class CompProperties_DTemperatureIngestible : CompProperties_DTemperature
    {

        public CompProperties_DTemperatureIngestible()
        {
            this.compClass = typeof(CompDTemperatureIngestible);
        }

        public bool isDrink = false;
        public bool okFrozen = false;
        public bool roomTemperature = false;  
    }
}
