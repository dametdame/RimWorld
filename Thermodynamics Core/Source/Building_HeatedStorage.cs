using DThermodynamicsCore.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DThermodynamicsCore
{
    class Building_HeatedStorage : Building_Storage
    {
        public CompPowerTrader compPowerTrader;
        public CompDTempControl compDTempControl;
        public CompRefuelable compRefuelable;

        public static float slowRotRateAmount = -99999f;
        public static float unrefrigeratedRotRate = -99999f;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.compPowerTrader = base.GetComp<CompPowerTrader>();
            this.compDTempControl = base.GetComp<CompDTempControl>();
            this.compRefuelable = base.GetComp<CompRefuelable>();
            if (unrefrigeratedRotRate < 0)
                unrefrigeratedRotRate = GenTemperature.RotRateAtTemperature(999f);
            if (slowRotRateAmount < 0)  
                slowRotRateAmount = unrefrigeratedRotRate - GenTemperature.RotRateAtTemperature(1f);
           
        }

        public virtual float GetTargetTemp()
        {
            if (compDTempControl == null)
            {
                Log.Error("Thermodynamics: No CompTempControl in Building_HeatedStorage, defName is " + this.def.defName);
                return 21f;
            }
            return compDTempControl.targetTemperature;
        }

        public virtual bool IsActive()
        {
            return (compPowerTrader != null && compPowerTrader.PowerOn || compRefuelable != null && compRefuelable.HasFuel || compPowerTrader == null && compRefuelable == null);
        }

        public override void Notify_ReceivedThing(Thing newItem)
        {
            base.Notify_ReceivedThing(newItem);
            newItem.Rotation = this.Rotation;
        }

        public virtual void ChangeHeat(int ticks)
        {
            if (!IsActive())
                return;

            float targetTemp = GetTargetTemp();

            foreach (Thing thing in base.slotGroup.HeldThings)
            {
                CompDTemperature comp = thing.TryGetComp<CompDTemperature>();
                if (comp != null && comp.curTemp < targetTemp)
                {
                    comp.Diffuse(targetTemp*2f, ticks);   
                }
                
                if (ThermodynamicsSettings.warmersSlowRot)
                {
                    CompRottable rot = thing.TryGetComp<CompRottable>();
                    if (rot != null && GenTemperature.RotRateAtTemperature(thing.AmbientTemperature) == unrefrigeratedRotRate)
                    {
                        rot.RotProgress = rot.RotProgress - ticks * slowRotRateAmount;
                    }
                }
            }
        }

        public override void Tick()
        {
            base.Tick();
            ChangeHeat(1);
        }

        public override void TickRare()
        {
            base.TickRare();
            ChangeHeat(250);
        }

    }
}
