using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DActiveCircuits
{
    class PowerMaterial : UnityEngine.Material
    {

        public PowerMaterial(Material source, CompPower c) : base(source)
        {
            comp = c;
        }

        public CompPower comp;
    }
}
