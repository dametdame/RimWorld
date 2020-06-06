using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;


namespace DActiveCircuits
{
    class SectionLayer_TopPowerGrid : SectionLayer_Things
    {
        public SectionLayer_TopPowerGrid(Section section) : base(section)
        {
            this.requireAddToMapMesh = false;
            this.relevantChangeTypes = (MapMeshFlag)32768
;
        }

        public override void DrawLayer()
        {
            if (ActiveCircuitsSettings.displayBadOnMain)
            {
                base.DrawLayer();
            }
        }

        protected override void TakePrintFrom(Thing t)
        {
            if (!ActiveCircuitsSettings.displayBadOnMain)
            {
                return;
            }
            if (t.Faction != null && t.Faction != Faction.OfPlayer)
            {
                return;
            }
            
            Building building = t as Building;
            if (building != null)
            {
                CompPower comp = t.TryGetComp<CompPower>();
                if (comp != null && ShouldPrint(comp))
                {
                    building.PrintForPowerGrid(this);
                }
            }
        }

        public bool ShouldPrint(CompPower comp)
        {
            Color color = ActiveCircuitsBase.GetNetColor(comp);
            return (color == ActiveCircuitsSettings.noPower || color == ActiveCircuitsSettings.nothingConnected || color == ActiveCircuitsSettings.justConduits);
        }
    }
}
