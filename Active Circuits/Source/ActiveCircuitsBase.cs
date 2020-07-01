using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Noise;

namespace DActiveCircuits
{
    public static class ActiveCircuitsBase
	{

		public static Dictionary<PowerNet, Color> netColors = new Dictionary<PowerNet, Color>();

		
		public static void DrawPowerGrid(SectionLayer layer, CompPower comp)
		{
			if (comp.TransmitsPowerNow)
			{
				//Graphic_LinkedTransmitterOverlay graphic = (Graphic_LinkedTransmitterOverlay)PowerOverlayMats.LinkedOverlayGraphic;
				//Color color = GetNetColor(comp);
				//SectionLayer l = layer;

				//Graphic_LinkedTransmitterOverlay colored = GetColoredVersion(graphic, graphic.Shader, color, color);
				//colored.Print(l, comp.parent);
				Graphic_LinkedTransmitterOverlay graphic = NewPowerOverlayMats.LinkedOverlayGraphic;
				graphic.Print(layer, comp.parent);
			}
			if (comp.parent.def.ConnectToPower)
			{
				PowerNetGraphics.PrintOverlayConnectorBaseFor(layer, comp.parent);
			}
			if (comp.connectParent != null)
			{
				PowerNetGraphics.PrintWirePieceConnecting(layer, comp.parent, comp.connectParent.parent, true);
			}
		}

		public static void UpdateAllNets(Map map)
		{
			netColors = new Dictionary<PowerNet, Color>();
			foreach (PowerNet net in map.powerNetManager.AllNetsListForReading)
			{
				SetNetColor(net);
			}
		
			FieldInfo sf = AccessTools.Field(typeof(MapDrawer), "sections");
			Section[,] sections = sf.GetValue(map.mapDrawer) as Section[,];
			if (sections != null)
			{
				foreach (Section s in sections)
				{
					SectionLayer pLayer = s.GetLayer(typeof(SectionLayer_ThingsPowerGrid));
					RegenerateColors(s, pLayer);
					if (ActiveCircuitsSettings.displayBadOnMain)
					{
						SectionLayer_TopPowerGrid l = (SectionLayer_TopPowerGrid)s.GetLayer(typeof(SectionLayer_TopPowerGrid));
						l.subMeshes.Clear();
						l.Regenerate();
						SectionLayer tpLayer = s.GetLayer(typeof(SectionLayer_TopPowerGrid));
						RegenerateColors(s, tpLayer);
					}
				}
			}
		}

		public static void RegenerateColors(Section sect, SectionLayer layer)
		{
			foreach (LayerSubMesh lsm in layer.subMeshes)
			{
				PowerMaterial mat = lsm.material as PowerMaterial;
				if (mat != null && mat.comp.PowerNet != null)
				{
					Color color = GetNetColor(mat.comp);
					mat.color = color;
				}
			}
		}

		public static void PowerMetrics(PowerNet net, out float production, out float consumption)
		{
			consumption = 0;
			production = 0;
			foreach (CompPowerTrader comp in net.powerComps)
			{
				if (comp.EnergyOutputPerTick > 0f)
					production += comp.EnergyOutputPerTick;
				else
					consumption += comp.EnergyOutputPerTick;
			}
		}


		public static void SetNetColor(PowerNet net)
		{
			if (net.powerComps.Count == 0 && net.batteryComps.Count == 0) // just conduits
			{
				netColors.SetOrAdd(net, ActiveCircuitsSettings.justConduits);
				return;
			}

			float production;
			float consumption;
			PowerMetrics(net, out production, out consumption);
		    if (consumption == 0) // nothing using power
			{
				netColors.SetOrAdd(net, ActiveCircuitsSettings.nothingConnected);
				return;
			}

			float rate = consumption + production;
			
			if (rate < 0)
			{
				float stored = net.CurrentStoredEnergy();
				if (stored < 5f)
					netColors.SetOrAdd(net, ActiveCircuitsSettings.noPower);
				else
					netColors.SetOrAdd(net, ActiveCircuitsSettings.drainingPower);
				return;
			}
			else // rate >= 0
			{
				if (rate > 1.5*consumption || net.CurrentStoredEnergy() >= consumption) // 1 day of power
					netColors.SetOrAdd(net, ActiveCircuitsSettings.surplusPower);
				else
					netColors.SetOrAdd(net, ActiveCircuitsSettings.hasPower);
				return;
			}
			
		}

		public static Color GetNetColor(CompPower comp)
		{
			Color result;
			return netColors.TryGetValue(comp.PowerNet, out result) ? result : Color.white;
		}

		public static Graphic SubGraphic(Graphic_Linked graphic)
		{
			FieldInfo subGraphic = AccessTools.Field(typeof(Graphic_Linked), "subGraphic");
			return (Graphic)subGraphic.GetValue(graphic);
		}

		public static Graphic_LinkedTransmitterOverlay GetColoredVersion(Graphic_LinkedTransmitterOverlay graphic, Shader newShader, Color newColor, Color newColorTwo)
		{
			return new Graphic_LinkedTransmitterOverlay(SubGraphic(graphic).GetColoredVersion(newShader, newColor, newColorTwo))
			{
				data = graphic.data
			};
		}

		public static Color salmon = new UnityEngine.Color(250f/255f, 127f/ 255f, 114f/ 255f);
		public static Color pink = new UnityEngine.Color(255f/255f, 204f/255f, 203f/255f);
		public static Color electricBlue = new UnityEngine.Color(77f/255f, 200f/255f, 232f/255f);
	}
}
