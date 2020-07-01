using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using RimWorld;
using Verse;
using DHotMeals.Comps;

namespace DHotMeals
{
    class JobDriver_HeatMeal : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if(!this.pawn.Reserve(this.job.GetTarget(TargetIndex.C), this.job, 1, -1, null, errorOnFailed))
            {
                return false;
            }
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
			yield return this.ReserveFood();
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
			yield return Toils_Ingest.PickupIngestible(TargetIndex.A, this.pawn);
			Toil empty = new Toil();
            yield return Toils_Jump.JumpIf(empty, delegate ()
            {
                LocalTargetInfo food = this.job.GetTarget(TargetIndex.A);
				CompDFoodTemperature comp = food.Thing.TryGetComp<CompDFoodTemperature>();
                if (comp == null)
                    return true;
                if (comp.PropsTemp.likesHeat)
                    return comp.curTemp >= comp.targetCookingTemp;
                return comp.curTemp > 0;
            });
			if (!HotMealsSettings.multipleHeat)
			{
				yield return Toils_Reserve.Reserve(TargetIndex.C);
				yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.InteractionCell).FailOnDestroyedOrNull(TargetIndex.C);
				yield return Toils_HeatMeal.HeatMeal().FailOnDespawnedNullOrForbiddenPlacedThings().FailOnCannotTouch(TargetIndex.C, PathEndMode.InteractionCell);
				yield return Toils_Reserve.Release(TargetIndex.C);
			}
            else
            {
				yield return Toils_Goto.GotoThing(TargetIndex.C, PathEndMode.Touch).FailOnDestroyedOrNull(TargetIndex.C);
				yield return Toils_HeatMeal.HeatMeal().FailOnDespawnedNullOrForbiddenPlacedThings().FailOnCannotTouch(TargetIndex.C, PathEndMode.Touch);
			}
            yield return empty;
            yield break;
        }


		public Toil ReserveFood()
		{
			return new Toil
			{
				initAction = delegate ()
				{
					if (this.pawn.Faction == null)
					{
						return;
					}
					Thing thing = this.job.GetTarget(TargetIndex.A).Thing;
					if (this.pawn.carryTracker.CarriedThing == thing)
					{
						return;
					}
					int maxAmountToPickup = FoodUtility.GetMaxAmountToPickup(thing, this.pawn, this.job.count);
					if (maxAmountToPickup == 0)
					{
						return;
					}
					if (!this.pawn.Reserve(thing, this.job, 10, maxAmountToPickup, null, true))
					{
						Log.Error(string.Concat(new object[]
						{
							"Pawn food reservation for ",
							this.pawn,
							" on job ",
							this,
							" failed, because it could not register food from ",
							thing,
							" - amount: ",
							maxAmountToPickup
						}), false);
						this.pawn.jobs.EndCurrentJob(JobCondition.Errored, true, true);
					}
					this.job.count = maxAmountToPickup;
				},
				defaultCompleteMode = ToilCompleteMode.Instant,
				atomicWithPrevious = true
			};
		}
	}
}
