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
            LocalTargetInfo food = jd.job.GetTarget(foodIndex);
            CompDFoodTemperature comp = food.Thing.TryGetComp<CompDFoodTemperature>();
            if (comp == null)
            {
                foreach(Toil value in values)
                {
                    yield return value;
                }
                yield break;
            }

            var enumerator = values.GetEnumerator();
            for (int i = 0; i < num; i++)
            {
                enumerator.MoveNext();
                yield return enumerator.Current;

            }

            foreach (Toil toil in Heat(jd, comp, foodIndex, finalLocation))
            {
                yield return toil;
            }


            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        public static IEnumerable<Toil> Heat(JobDriver jd, CompDFoodTemperature comp, TargetIndex foodIndex = TargetIndex.A, TargetIndex finalLocation = TargetIndex.C)
        {
            
            LocalTargetInfo oldFinal = jd.job.GetTarget(finalLocation);
            
            if (comp.PropsTemp.likesHeat || (HotMealsSettings.thawIt && !comp.PropsTemp.okFrozen))
            {
                Toil empty = new Toil();
                yield return Toils_Jump.JumpIf(empty, delegate ()
                {
                    if (comp.PropsTemp.likesHeat)
                        return comp.curTemp >= comp.PropsTemp.tempLevels.goodTemp;
                    return comp.curTemp > 0;
                });
                Toil getHeater = new Toil();
                getHeater.initAction = delegate ()
                {
                    Pawn actor = getHeater.actor;
                    Job curJob = actor.jobs.curJob;
                    Thing foodToHeat = curJob.GetTarget(foodIndex).Thing;
                    Thing heater = Toils_HeatMeal.FindPlaceToHeatFood(foodToHeat, actor);
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
                yield return Toils_Reserve.Reserve(finalLocation);
                yield return Toils_Goto.GotoThing(finalLocation, PathEndMode.InteractionCell);
                yield return Toils_HeatMeal.HeatMeal(foodIndex, finalLocation).FailOnDespawnedNullOrForbiddenPlacedThings().FailOnCannotTouch(finalLocation, PathEndMode.InteractionCell); ;
                yield return Toils_Reserve.Release(finalLocation);
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
}
