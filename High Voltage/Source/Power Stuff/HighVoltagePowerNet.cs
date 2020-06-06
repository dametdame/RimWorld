using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace DHighVoltage
{
	[StaticConstructorOnStartup]
    public class HighVoltagePowerNet : PowerNet
    {
        public static FieldInfo pwpo;
        public static FieldInfo psp;

        public static MethodInfo cste;

		static HighVoltagePowerNet()
		{
			HighVoltagePowerNet.pwpo = AccessTools.Field(typeof(PowerNet), "partsWantingPowerOn");
			HighVoltagePowerNet.psp = AccessTools.Field(typeof(PowerNet), "potentialShutdownParts");
			HighVoltagePowerNet.cste = AccessTools.Method(typeof(PowerNet), "ChangeStoredEnergy");
		}

        public HighVoltagePowerNet(IEnumerable<CompPower> newTransmitters) : base(newTransmitters) 
        {
			double? avgConductivity = this.transmitters.Average(delegate (CompPower cp)
			{
				CompDVoltage comp = cp.parent.GetComp<CompDVoltage>();
				return (comp != null) ? comp.capacityMult : null as double?;
			});
			netConductivity = avgConductivity ?? -1f;
			double? avgDurability = this.transmitters.Average(delegate (CompPower cp)
			{
				CompDVoltage comp = cp.parent.GetComp<CompDVoltage>();
				return (comp != null) ? comp.outputMult : null as double?;
			});
			netDurability = avgDurability ?? -1f;
		}

        public static List<CompPowerTrader> partsWantingPowerOn
        {
            get
            {
                return pwpo.GetValue(null) as List<CompPowerTrader>;
            }
        }

        public static List<CompPowerTrader> potentialShutdownParts
        {
            get
            {
                return psp.GetValue(null) as List<CompPowerTrader>;
            }
        }

        public void ChangeStoredEnergy(float extra)
        {
            cste.Invoke(this, new object[] { extra });
        }

		public void GetEnergyRates(out float generated, out float consumed)
		{
			generated = 0f;
			consumed = 0f;
			if (DebugSettings.unlimitedPower)
			{
				generated = 100000f;
				consumed = 0f;
				return;
			}
			foreach (CompPowerTrader comp in this.powerComps)
			{
				if (comp.PowerOn)
				{
					if(comp.EnergyOutputPerTick > 0)
					{
						generated += comp.EnergyOutputPerTick;
					}
					else
					{
						consumed += comp.EnergyOutputPerTick;
					}
				}
			}
		}

        public new void PowerNetTick()
        {

			List<CompPowerTrader> partsWantingPowerOn = HighVoltagePowerNet.partsWantingPowerOn;

			float consumed;
			float generated;
			this.GetEnergyRates(out generated, out consumed);
			float gainRate = consumed + generated;
			float storedEnergy = this.CurrentStoredEnergy();
			if (storedEnergy + gainRate >= -1E-07f && !this.Map.gameConditionManager.ElectricityDisabled)
			{
				float num3;
				if (this.batteryComps.Count > 0 && storedEnergy >= 0.1f)
				{
					num3 = storedEnergy - 5f;
				}
				else
				{
					num3 = storedEnergy;
				}
				if (num3 + gainRate >= 0f)
				{
					partsWantingPowerOn.Clear();
					for (int i = 0; i < this.powerComps.Count; i++)
					{
						if (!this.powerComps[i].PowerOn && FlickUtility.WantsToBeOn(this.powerComps[i].parent) && !this.powerComps[i].parent.IsBrokenDown())
						{
							partsWantingPowerOn.Add(this.powerComps[i]);
						}
					}
					if (partsWantingPowerOn.Count > 0)
					{
						int num4 = 200 / partsWantingPowerOn.Count;
						if (num4 < 30)
						{
							num4 = 30;
						}
						if (Find.TickManager.TicksGame % num4 == 0)
						{
							int num5 = Mathf.Max(1, Mathf.RoundToInt((float)partsWantingPowerOn.Count * 0.05f));
							for (int j = 0; j < num5; j++)
							{
								CompPowerTrader compPowerTrader = partsWantingPowerOn.RandomElement<CompPowerTrader>();
								if (!compPowerTrader.PowerOn && gainRate + storedEnergy >= -(compPowerTrader.EnergyOutputPerTick + 1E-07f))
								{
									compPowerTrader.PowerOn = true;
									gainRate += compPowerTrader.EnergyOutputPerTick;
								}
							}
						}
					}
				}
				this.ChangeStoredEnergy(gainRate);
				return;
			}
			if (Find.TickManager.TicksGame % 20 == 0)
			{
				List<CompPowerTrader> potentialShutdownParts = HighVoltagePowerNet.potentialShutdownParts;
				potentialShutdownParts.Clear();
				for (int k = 0; k < this.powerComps.Count; k++)
				{
					if (this.powerComps[k].PowerOn && this.powerComps[k].EnergyOutputPerTick < 0f)
					{
						potentialShutdownParts.Add(this.powerComps[k]);
					}
				}
				if (potentialShutdownParts.Count > 0)
				{
					int num6 = Mathf.Max(1, Mathf.RoundToInt((float)potentialShutdownParts.Count * 0.05f));
					for (int l = 0; l < num6; l++)
					{
						potentialShutdownParts.RandomElement<CompPowerTrader>().PowerOn = false;
					}
				}
			}
			return;
		}

		public double netConductivity;
		public double netDurability;

		public double netBaseCapacity;

	}
}
