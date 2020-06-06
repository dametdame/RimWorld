using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using System.Reflection;

namespace DHighVoltage
{
    [HarmonyPatch(typeof(PowerNet))]
    [HarmonyPatch("PowerNetTick")]
    class Patch_PowerNetTick_Prefix
    {
        public static bool Prefix(PowerNet __instance)
        {
			HighVoltagePowerNet hvpn = __instance as HighVoltagePowerNet;
            hvpn.PowerNetTick();
            return false;
        }
    }
}
