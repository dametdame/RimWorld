using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using DThermodynamicsCore.Comps;

namespace DHotMeals.Comps
{
    public class CompDFoodTemperature : CompDTemperatureIngestible
    {
		public new virtual CompProperties_DFoodTemperature PropsTemp
		{
			get
			{
				return this.props as CompProperties_DFoodTemperature;
			}
		}

		public virtual void SetHeatUpRate(Thing heater)
		{
			float workRate = heater.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor);
			if (workRate > 0)
			{
				this.heatUpRate = 0.25 * workRate * HotMealsSettings.heatSpeedMult;
			}
			else
				this.heatUpRate = 0.125 * HotMealsSettings.heatSpeedMult;
		}

		public virtual void HeatUp()
		{
			this.curTemp += heatUpRate;
		}

		public string GetFoodType()
		{
			if (PropsTemp.displayName != null)
				return PropsTemp.displayName;
			string s = "";
			if (this.PropsTemp.roomTemperature)
			{
				s += "Lukewarm";
			}
			else if (this.PropsTemp.likesHeat)
			{
				s += "Hot";
			}
			else
			{
				s += "Cold";
			}
			s += " ";
			if (this.PropsTemp.isDrink)
			{
				s += "drink";
			}
			else
			{
				s += "meal";
			}
			return s;
		}

		public int GetPositiveMoodEffect(double temp)
		{
			if (this.PropsTemp.roomTemperature)
			{
				if (temp >= Levels.goodTemp && temp < Levels.okTemp)
					return 1;
			}
			else if (this.PropsTemp.likesHeat)
			{
				if (temp > Levels.goodTemp)
					return 1;
			}
			else
			{
				if (temp < 0 && !this.PropsTemp.okFrozen)
					return 0;
				else if (temp < Levels.goodTemp)
					return 1;
			}
			return 0;
		}

		public int GetNegativeMoodEffect(double temp)
		{
			if (this.PropsTemp.roomTemperature)
			{
				if (temp < 0 && !this.PropsTemp.okFrozen)
					return -3;
				else if (temp < Levels.goodTemp) // too cold
					return -1;
				else if (temp < Levels.okTemp) // between good (lower bound) and ok (upper bound); just right
					return 0;
				else // too hot
					return 1;
			}
			else if (this.PropsTemp.likesHeat)
			{
				if (temp < 0 && !this.PropsTemp.okFrozen)
					return -3;
				if (temp > Levels.okTemp)
					return 0;
				else if (temp > Levels.badTemp)
					return -1;
				else if (temp > Levels.reallyBadTemp)
					return -2;
				else // colder than reallyBadTemp (default: 0.01 celsius)
					return -3;
			}
			else
			{
				if (temp < 0 && !this.PropsTemp.okFrozen)
					return -3;
				else if (temp < Levels.okTemp)
					return 0;
				else if (temp < Levels.badTemp)
					return 1;
				else if (temp < Levels.reallyBadTemp)
					return 2;
				else // hotter than really bad temp
					return 3;
			}
		}


		public override string GetState(double temp)
		{
			if (PropsTemp.noHeat) 
			{
				if (curTemp > 0)
					return "edible";
				return "frozen solid";
			}

			if (GetPositiveMoodEffect(temp) == 1)
				return "just right";

			int negLevel = GetNegativeMoodEffect(temp);
			if (negLevel == -3)
				return "frozen solid";
			else if (negLevel == -2)
				return "too cold";
			else if (negLevel == -1)
				return "too chilly";
			else if (negLevel == 0)
				return "edible";
			else if (negLevel == 1)
				return "too warm";
			else if (negLevel == 2)
				return "too hot";
			else if (negLevel == 3)
				return "scalding";
			return "";
		}

		public override ThoughtDef GetIngestMemory()
		{
			if (GetPositiveMoodEffect(this.curTemp) > 0)
				return Base.DefOf.DAteGoodThing;
			int negEffect = GetNegativeMoodEffect(this.curTemp);
			if (negEffect < 0)
				return Base.DefOf.DAteTooCold;
			else if (negEffect > 0)
				return Base.DefOf.DAteTooHot;
			else
				return Base.DefOf.DAteMeh;
		}

		public override void MakeIngestMemory(ThoughtDef memory, Pawn ingester)
		{
			Thought_MealTemp foodMem = (Thought_MealTemp)ThoughtMaker.MakeThought(memory);
			foodMem.createdTemp = curTemp;
			foodMem.comp = this;
			ingester.needs.mood.thoughts.memories.TryGainMemory(foodMem, null);
		}

		public override void Initialize(CompProperties props)
		{
			base.Initialize(props);
			if (PropsTemp.roomTemperature)
				targetCookingTemp = PropsTemp.tempLevels.okTemp - 5;
			else if (PropsTemp.likesHeat)
				targetCookingTemp = PropsTemp.tempLevels.goodTemp + 20;
			else
				targetCookingTemp = 0;
		}

		public override string CompInspectStringExtra()
		{
			string s = this.GetFoodType() + ", ";
			return s += base.CompInspectStringExtra();
		}

		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			JobFailReason.Clear();
			if ((!PropsTemp.likesHeat && (!HotMealsSettings.thawIt || PropsTemp.okFrozen) || !selPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) || this.parent.stackCount <= 0))
				yield break;

			Thing heater = null;
			if (curTemp >= PropsTemp.tempLevels.goodTemp + 5)
			{
				JobFailReason.Is("alreadyHeated".Translate(), null);
			}
			else if ((heater = Toils_HeatMeal.FindPlaceToHeatFood(this.parent, selPawn, 9999)) == null)
			{
				JobFailReason.Is("noPlaceToHeat".Translate(), null);
			}
			else if (!selPawn.CanReserve(this.parent, 1, -1, null, false))
			{
				Pawn pawn = selPawn.Map.reservationManager.FirstRespectedReserver(this.parent, selPawn);
				if (pawn == null)
				{
					pawn = selPawn.Map.physicalInteractionReservationManager.FirstReserverOf(selPawn);
				}
				if (pawn != null)
				{
					JobFailReason.Is("ReservedBy".Translate(pawn.LabelShort, pawn), null);
				}
				else
				{
					JobFailReason.Is("Reserved".Translate(), null);
				}
			}


			Job job = null;
			if (heater != null && this.parent.stackCount > 0)
			{
				job = JobMaker.MakeJob(Base.DefOf.HeatMeal);
				job.targetA = this.parent;
				job.targetC = heater;
				job.count = this.parent.stackCount;
			}
			if (JobFailReason.HaveReason)
			{
				string command;
				if (PropsTemp.likesHeat)
					command = "CannotGenericWorkCustom".Translate("heatMeal".Translate(this.parent.Label));
				else
					command = "CannotGenericWorkCustom".Translate("thawMeal".Translate(this.parent.Label));
				command += ": " + JobFailReason.Reason.CapitalizeFirst();
				yield return new FloatMenuOption(command, null, MenuOptionPriority.Default, null, null, 0f, null, null);
				JobFailReason.Clear();
			}
			else if (job != null)
			{
				string command;
				if (PropsTemp.likesHeat)
					command = "heatMeal".Translate(this.parent.Label).CapitalizeFirst();
				else
					command = "thawMeal".Translate(this.parent.Label).CapitalizeFirst();
				yield return new FloatMenuOption(command, delegate ()
				{
					selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
				}, MenuOptionPriority.Default, null, null, 0f, null, null);
			}
			yield break;


		}

		public double initialHeatingTemp = 0f;
		public double targetCookingTemp;
		public double heatUpRate = 0.1;

	}
}
