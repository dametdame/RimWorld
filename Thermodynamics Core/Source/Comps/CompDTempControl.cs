﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;

// basically the same as CompTempControl, but they made it annoying to inherit and modify

namespace DThermodynamicsCore.Comps
{
    public class CompDTempControl : ThingComp
    {
		[Unsaved(false)]
		public bool operatingAtHighPower;
		public float targetTemperature = -99999f;
		private const float DefaultTargetTemperature = 21f;

		public CompProperties_DTempControl Props
		{
			get
			{
				return (CompProperties_DTempControl)this.props;
			}
		}

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (this.targetTemperature < -2000f)
			{
				this.targetTemperature = this.Props.defaultTargetTemperature;
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<float>(ref this.targetTemperature, "targetTemperature", 0f, false);
		}

		private float RoundedToCurrentTempModeOffset(float celsiusTemp)
		{
			return GenTemperature.ConvertTemperatureOffset((float)Mathf.RoundToInt(GenTemperature.CelsiusToOffset(celsiusTemp, Prefs.TemperatureMode)), Prefs.TemperatureMode, TemperatureDisplayMode.Celsius);
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo gizmo in base.CompGetGizmosExtra())
			{
				yield return gizmo;
			}
			float offset2 = this.RoundedToCurrentTempModeOffset(-10f);
			yield return new Command_Action
			{
				action = delegate ()
				{
					this.InterfaceChangeTargetTemperature(offset2);
				},
				defaultLabel = offset2.ToStringTemperatureOffset("F0"),
				defaultDesc = "CommandLowerTempDesc".Translate(),
				hotKey = KeyBindingDefOf.Misc5,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower", true)
			};
			float offset3 = this.RoundedToCurrentTempModeOffset(-1f);
			yield return new Command_Action
			{
				action = delegate ()
				{
					this.InterfaceChangeTargetTemperature(offset3);
				},
				defaultLabel = offset3.ToStringTemperatureOffset("F0"),
				defaultDesc = "CommandLowerTempDesc".Translate(),
				hotKey = KeyBindingDefOf.Misc4,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/TempLower", true)
			};
			yield return new Command_Action
			{
				action = delegate ()
				{
					this.targetTemperature = 21f;
					SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
					this.ThrowCurrentTemperatureText();
				},
				defaultLabel = "CommandResetTemp".Translate(),
				defaultDesc = "CommandResetTempDesc".Translate(),
				hotKey = KeyBindingDefOf.Misc1,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/TempReset", true)
			};
			float offset4 = this.RoundedToCurrentTempModeOffset(1f);
			yield return new Command_Action
			{
				action = delegate ()
				{
					this.InterfaceChangeTargetTemperature(offset4);
				},
				defaultLabel = "+" + offset4.ToStringTemperatureOffset("F0"),
				defaultDesc = "CommandRaiseTempDesc".Translate(),
				hotKey = KeyBindingDefOf.Misc2,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise", true)
			};
			float offset = this.RoundedToCurrentTempModeOffset(10f);
			yield return new Command_Action
			{
				action = delegate ()
				{
					this.InterfaceChangeTargetTemperature(offset);
				},
				defaultLabel = "+" + offset.ToStringTemperatureOffset("F0"),
				defaultDesc = "CommandRaiseTempDesc".Translate(),
				hotKey = KeyBindingDefOf.Misc3,
				icon = ContentFinder<Texture2D>.Get("UI/Commands/TempRaise", true)
			};
			yield break;
		}
		private void InterfaceChangeTargetTemperature(float offset)
		{
			SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
			this.targetTemperature += offset;
			this.targetTemperature = Mathf.Clamp(this.targetTemperature, Props.minTargetTemperature, Props.maxTargetTemperature);
			this.ThrowCurrentTemperatureText();
		}

		private void ThrowCurrentTemperatureText()
		{
			MoteMaker.ThrowText(this.parent.TrueCenter() + new Vector3(0.5f, 0f, 0.5f), this.parent.Map, this.targetTemperature.ToStringTemperature("F0"), Color.white, -1f);
		}

		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("TargetTemperature".Translate() + ": ");
			stringBuilder.Append(this.targetTemperature.ToStringTemperature("F0"));
			return stringBuilder.ToString();
		}
	}
}
