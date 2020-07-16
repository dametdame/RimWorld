using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using DThermodynamicsCore.Comps;

namespace DThermodynamicsCore.Core_Patches
{
    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("AmbientTemperature", MethodType.Getter)]
    class Patch_ThingAmbientTemperature_Prefix
    {
        public static bool Prefix(ref float __result, Thing __instance)
        {
            CompDTemperature comp = __instance.TryGetComp<CompDTemperature>();
            if (comp != null)
            {
                __result = comp.GetCurTemp();
                return false;
            }
            return true;
        }
    }
}
