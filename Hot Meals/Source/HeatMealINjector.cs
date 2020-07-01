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
    public static class HeatMealInjector
    {

        public static IEnumerable<Toil> InjectHeat(IEnumerable<Toil> values, JobDriver jd, int num, TargetIndex foodIndex = TargetIndex.A, TargetIndex finalLocation = TargetIndex.C)
        {
            var enumerator = values.GetEnumerator();
            for (int i = 0; i < num; i++)
            {
                enumerator.MoveNext();
                yield return enumerator.Current;

            }

            foreach (Toil toil in Heat(jd, foodIndex, finalLocation))
            {
                yield return toil;
            }


            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        public static IEnumerable<Toil> Heat(JobDriver jd, TargetIndex foodIndex = TargetIndex.A, TargetIndex finalLocation = TargetIndex.C, TargetIndex tableIndex = TargetIndex.None)
        {
            
            LocalTargetInfo oldFinal = jd.job.GetTarget(finalLocation);
            
            Toil empty = new Toil();
            yield return Toils_Jump.JumpIf(empty, delegate ()
            {
                Pawn actor = empty.actor;
                Job curJob = actor.jobs.curJob;
                LocalTargetInfo food = curJob.GetTarget(foodIndex).Thing;
                if (food.Thing == null)
                    return true;
                CompDFoodTemperature comp = food.Thing.TryGetComp<CompDFoodTemperature>();
                if (comp == null)
                    return true;
                if (comp.PropsTemp.likesHeat)
                    return comp.curTemp >= comp.PropsTemp.tempLevels.goodTemp;
                else if (HotMealsSettings.thawIt && !comp.PropsTemp.okFrozen)
                    return comp.curTemp > 0;
                return true;
            });
            Toil getHeater = new Toil();
            getHeater.initAction = delegate ()
            {
                Pawn actor = getHeater.actor;
                Job curJob = actor.jobs.curJob;
                Thing foodToHeat = curJob.GetTarget(foodIndex).Thing;
                Thing table = null;
                if (tableIndex != TargetIndex.None)
                     table = curJob.GetTarget(tableIndex).Thing;
                Thing heater = Toils_HeatMeal.FindPlaceToHeatFood(foodToHeat, actor, searchNear: table);
                if (heater != null)
                {
                    curJob.SetTarget(finalLocation, heater);
                }
            };
            yield return getHeater;
            yield return Toils_Jump.JumpIf(empty, delegate ()
            {
                Pawn actor = getHeater.actor;
                Job curJob = actor.jobs.curJob;
                Thing heater = curJob.GetTarget(finalLocation).Thing;
                return (heater == null);
            });
            if (!HotMealsSettings.multipleHeat)
            {
                yield return Toils_Reserve.Reserve(finalLocation);
                yield return Toils_Goto.GotoThing(finalLocation, PathEndMode.InteractionCell);
                yield return Toils_HeatMeal.HeatMeal(foodIndex, finalLocation).FailOnDespawnedNullOrForbiddenPlacedThings().FailOnCannotTouch(finalLocation, PathEndMode.InteractionCell);
                yield return Toils_Reserve.Release(finalLocation);
            }
            else
            {
                yield return Toils_Goto.GotoThing(finalLocation, PathEndMode.Touch);
                yield return Toils_HeatMeal.HeatMeal(foodIndex, finalLocation).FailOnDespawnedNullOrForbiddenPlacedThings().FailOnCannotTouch(finalLocation, PathEndMode.Touch);
            }
            yield return empty;
            if (oldFinal != LocalTargetInfo.Invalid)
            {
                Toil resetC = new Toil();
                resetC.initAction = delegate ()
                {
                    Pawn actor = resetC.actor;
                    Job curJob = actor.jobs.curJob;
                    curJob.SetTarget(finalLocation, oldFinal);
                };
                yield return resetC;
            }
            
        }
    }
}
