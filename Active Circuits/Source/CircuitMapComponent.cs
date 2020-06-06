using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;


namespace DActiveCircuits
{
    public class CircuitMapComponent : MapComponent
    {
        Map thisMap;

        public CircuitMapComponent(Map map) : base(map) { thisMap = map; }

        public override void MapGenerated()
        {
            base.MapGenerated();
            ActiveCircuitsBase.UpdateAllNets(thisMap);
        }
    }
}
