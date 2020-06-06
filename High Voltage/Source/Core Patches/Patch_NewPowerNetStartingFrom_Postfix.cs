using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using System.Reflection;

namespace DHighVoltage.Core_Patches
{
    [HarmonyPatch(typeof(PowerNetMaker))]
    [HarmonyPatch("NewPowerNetStartingFrom")]
    class Patch_NewPowerNetStartingFrom_Postfix
    {

        public static bool Prefix(Building root, ref PowerNet __result)
        {
            MethodInfo ContiguousPowerBuildings = AccessTools.Method(typeof(PowerNetMaker), "ContiguousPowerBuildings");
            IEnumerable<CompPower> comps = (IEnumerable < CompPower > )ContiguousPowerBuildings.Invoke(null, new object[] { root });
            __result = new HighVoltagePowerNet(comps);
            return false;
        }
    }
}
