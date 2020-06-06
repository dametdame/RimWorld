using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace DThermodynamicsCore.Comps
{
    public class CompProperties_DTemperature : CompProperties
    {
        public CompProperties_DTemperature()
        {
            this.compClass = typeof(CompDTemperature);
            if(this.tickRate == -1)
            {
                this.tickRate = Rand.RangeInclusive(200, 250);
            }
        }

        public float initialTemp = 21f; // 70 fahrenheit
        public double diffusionTime = 5000; 
        public bool likesHeat = true;
        public DTemperatureLevels tempLevels = new DTemperatureLevels(); // ~100 fahrenheit

        public int tickRate = -1;
    }

    public class DTemperatureLevels
    {
        public DTemperatureLevels(float good = 37f, float ok = 20f, float bad = 10f, float reallyBad = 0.01f, float frozen = 0f)
        {
            goodTemp = good;
            okTemp = ok;
            badTemp = bad;
            reallyBadTemp = reallyBad;
            frozenTemp = frozen;
        }

        public float goodTemp = 37f;
        public float okTemp = 20f;
        public float badTemp = 10f;
        public float reallyBadTemp = 0.01f;
        public float frozenTemp = -500f;
    } 

}
