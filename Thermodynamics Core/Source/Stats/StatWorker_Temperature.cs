using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using DThermodynamicsCore.Comps;

namespace DThermodynamicsCore.Stats
{
    class StatWorker_Temperature : StatWorker
    {

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (!req.HasThing)
                return -9999;
            CompDTemperature comp = req.Thing.TryGetComp<CompDTemperature>();
            if (comp != null)
                return (float)comp.curTemp;
            return -9999;
        }
        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            return "";
        }

        public static string InIdealRange()
        {
            return "\nIn ideal temperature range";
        }

        public static string WillNeverReach()
        {
            return "\nWill currently never reach good temperature for consumption.";
        }

        public static string HoursToIdeal(CompDTemperatureIngestible comp, double idealTemp)
        {
            return "\nWill reach ideal temperature for consumption in around " + ThermodynamicsBase.HoursToTargetTemp(comp, idealTemp).ToString("f1") + " hours.";
        }

        public static string HoursInIdeal(CompDTemperatureIngestible comp, double changeThreshold)
        {
            return "\nWill remain in ideal range for around " + ThermodynamicsBase.HoursToTargetTemp(comp, changeThreshold).ToString("f1") + " hours more.";
        }

        public static string HoursToAmbient(CompDTemperature comp)
        {
            return "\nWill reach ambient temperature of " + GenText.ToStringTemperature((float)comp.AmbientTemperature) + " in around " + ThermodynamicsBase.HoursToTargetTemp(comp, comp.AmbientTemperature, 2).ToString("f1") + " hour(s).";
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            string s = "";
            if (!req.HasThing)
                return s;
            CompDTemperature comp = req.Thing.TryGetComp<CompDTemperature>();
            if (comp != null)
            {
                double interval = 2500;
                double ambient = comp.AmbientTemperature;
                double shift = ambient - comp.curTemp;
                double changeMag = Math.Abs(interval * shift / comp.PropsTemp.diffusionTime);
                double minStepScaled = CompDTemperature.minStep * interval;
                double step = (Math.Abs(shift) < minStepScaled || changeMag > CompDTemperature.minStep) ? changeMag : minStepScaled;
                if (step == minStepScaled)
                {
                    s += "Currently coming to equilibrium at " + GenText.ToStringTemperatureOffset((float)ambient);
                }
                else
                {
                    double result = Math.Sign(shift) * step * ThermodynamicsSettings.diffusionModifier;
                    s += "Currently diffusing at a rate of " + GenText.ToStringTemperatureOffset((float)result) + " per hour.";
                    s += HoursToAmbient(comp);
                    CompDTemperatureIngestible comp2 = comp as CompDTemperatureIngestible;
                    if (comp2 != null)
                    {
                        s += "\n";

                        if (comp2.PropsTemp.roomTemperature)
                        {
                            if (comp2.curTemp > comp2.PropsTemp.tempLevels.goodTemp && comp2.curTemp < comp2.PropsTemp.tempLevels.okTemp)
                            {
                                if (ambient < comp2.PropsTemp.tempLevels.goodTemp)
                                    s += HoursInIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                                else if (ambient > comp2.PropsTemp.tempLevels.okTemp)
                                    s += HoursInIdeal(comp2, comp2.PropsTemp.tempLevels.okTemp);
                                else
                                    s += InIdealRange();
                            }
                            else if (comp2.curTemp < comp2.PropsTemp.tempLevels.goodTemp && ambient > comp2.PropsTemp.tempLevels.goodTemp)
                                s += HoursToIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                            else if (comp2.curTemp > comp2.PropsTemp.tempLevels.okTemp && ambient < comp2.PropsTemp.tempLevels.okTemp)
                                s += HoursToIdeal(comp2, comp2.PropsTemp.tempLevels.okTemp);
                            else
                                s += WillNeverReach();
                        }
                        else if (comp2.PropsTemp.likesHeat)
                        {
                            if (comp2.curTemp > comp2.PropsTemp.tempLevels.goodTemp)
                            {
                                if (ambient < comp2.PropsTemp.tempLevels.goodTemp)
                                    s += HoursInIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                                else
                                    s += InIdealRange();
                            }
                            else if (ambient > comp2.PropsTemp.tempLevels.goodTemp)
                                s += HoursToIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                            else
                                s += WillNeverReach();
                        }
                        else
                        {
                            if (comp2.curTemp < 0f && !comp2.PropsTemp.okFrozen)
                            {
                                if (comp2.AmbientTemperature > 0f)
                                    s += HoursToIdeal(comp2, 0);
                                else if (comp2.AmbientTemperature <= 0f)
                                    s += WillNeverReach();
                            }
                            else if (comp2.curTemp < comp2.PropsTemp.tempLevels.goodTemp)
                            {
                                if (ambient > comp2.PropsTemp.tempLevels.goodTemp)
                                    s += HoursInIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                                else
                                    s += InIdealRange();
                            }
                            else if (ambient < comp2.PropsTemp.tempLevels.goodTemp)
                                s += HoursToIdeal(comp2, comp2.PropsTemp.tempLevels.goodTemp);
                            else
                                s += WillNeverReach();
                        }
                    }
                }
            }
            return s;
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            string s = GenText.ToStringTemperature(value);
            if (!optionalReq.HasThing)
                return s;
            CompDTemperature comp = optionalReq.Thing.TryGetComp<CompDTemperature>();
            if (comp != null)
                s += ", " + comp.GetState(comp.curTemp);
            return s;
        }
    }
}
