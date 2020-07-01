using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using UnityEngine;
using DHotMeals.Comps;

namespace DHotMeals
{
    public static class Toils_HeatMeal
    {
		public static List<ThingDef> allowedHeaters = new List<ThingDef> { Base.DefOf.DMicrowave};

        public static Thing FindPlaceToHeatFood(Thing food, Pawn pawn, float searchRadius = -1f, Thing searchNear = null)
        {
			if (searchNear == null)
				searchNear = food;

			if (searchRadius < 1f)
			{
				searchRadius = HotMealsSettings.searchRadius;
			}

			Predicate<Thing> valid = delegate (Thing m)
			{
				if (m.def.building == null || !m.def.building.isMealSource)
					return false;
				if (!HotMealsSettings.useCookingAppliances && !allowedHeaters.Contains(m.def))
					return false;
				CompRefuelable compRefuelable = m.TryGetComp<CompRefuelable>();
				if (compRefuelable != null && !compRefuelable.HasFuel)
					return false;
				CompPowerTrader compPowerTrader = m.TryGetComp<CompPowerTrader>();
				if (compPowerTrader != null && !compPowerTrader.PowerOn)
					return false;
				return !m.IsForbidden(pawn) && (HotMealsSettings.multipleHeat || pawn.CanReserve(m, 1, 1, null, false));
			};

			Thing result = GenClosest.ClosestThing_Regionwise_ReachablePrioritized(searchNear.PositionHeld, searchNear.MapHeld, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), 
				PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), searchRadius, valid, (thing) => thing.GetStatValue(StatDefOf.WorkTableWorkSpeedFactor));
			return result;
		}

		public static Toil HeatMeal(TargetIndex foodIndex = TargetIndex.A, TargetIndex heaterIndex = TargetIndex.C)
		{
			Toil toil = new Toil();
			toil.initAction = delegate ()
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				Thing food = curJob.GetTarget(foodIndex).Thing;
				CompDFoodTemperature comp = food.TryGetComp<CompDFoodTemperature>();
				if (comp == null)
				{
					Log.Error("Tried to heat food with no temperatureingestible comp");
					return;
				}
				Thing heater = curJob.GetTarget(heaterIndex).Thing;
				comp.initialHeatingTemp = comp.curTemp;
				comp.SetHeatUpRate(heater);
			};

			toil.tickAction = delegate ()
			{
				Pawn actor = toil.actor;
				Job curJob = actor.jobs.curJob;
				Thing food = curJob.GetTarget(foodIndex).Thing;
				CompDFoodTemperature comp = food.TryGetComp<CompDFoodTemperature>();
				if (comp == null)
				{
					Log.Error("Tried to heat food with no temperature ingestible comp");
					actor.jobs.curDriver.ReadyForNextToil();
					return;
				}
				
				if (comp.curTemp >= comp.targetCookingTemp)
				{
					actor.jobs.curDriver.ReadyForNextToil();
					return;
				}
				IBillGiverWithTickAction billGiverWithTickAction = toil.actor.CurJob.GetTarget(heaterIndex).Thing as IBillGiverWithTickAction;
				if (billGiverWithTickAction != null)
				{
					billGiverWithTickAction.UsedThisTick();
				}
				actor.GainComfortFromCellIfPossible(true);
				comp.HeatUp();	
			};
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			EffecterDef cook = DefDatabase<EffecterDef>.GetNamed("Cook");
			toil.WithEffect(() => cook, foodIndex);
			SoundDef cooking = DefDatabase<SoundDef>.GetNamed("Recipe_CookMeal");
			toil.PlaySustainerOrSound(() => cooking);
			toil.WithProgressBar(foodIndex, delegate
			{
				Pawn actor = toil.actor;
				Job curJob = actor.CurJob;
				Thing food = curJob.GetTarget(foodIndex).Thing;
				CompDFoodTemperature comp = food.TryGetComp<CompDFoodTemperature>();
				if (comp == null)
				{
					return 1f;
				}
				double target = comp.targetCookingTemp - comp.initialHeatingTemp;
				double progress = comp.curTemp - comp.initialHeatingTemp;
				return (float)(progress / target);

			}, false, -0.5f);
			toil.activeSkill = (() => SkillDefOf.Cooking);
			return toil;
		}

	}
}
