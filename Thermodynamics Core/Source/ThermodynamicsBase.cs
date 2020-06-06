using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using DThermodynamicsCore.Comps;

namespace DThermodynamicsCore
{
    [StaticConstructorOnStartup]
    public static class ThermodynamicsBase
    {
        public static float HoursToTargetTemp(CompDTemperature comp, double target, double tolerance = 0.5)
        {
            const double interval = 250;
            double ambient = comp.AmbientTemperature;
            double curtemp = comp.curTemp;
            bool goUp = curtemp < target;
            double minStepScaled = CompDTemperature.minStep * interval;
            double diffusionTime = comp.PropsTemp.diffusionTime;
            int count = 0;
            while (goUp ? (curtemp + tolerance < target) : (curtemp > target + tolerance) && count < 1000)
            {
                double shift = ambient - curtemp;
                double changeMag = Math.Abs(interval * shift / diffusionTime);
                double step = (Math.Abs(shift) < minStepScaled || changeMag > CompDTemperature.minStep) ? changeMag : minStepScaled;
                curtemp += Math.Sign(shift) * step * ThermodynamicsSettings.diffusionModifier;
                //curtemp += CompDTemperature.CalcTempChange(ambient, curtemp, interval, diffusionTime, minStepScaled);
                count++;
            }
            return (float)count / 10f;
        }


        static ThermodynamicsBase()
        { }

    }
}
