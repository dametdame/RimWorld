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
    [HarmonyPatch(typeof(CompRottable))]
    [HarmonyPatch("CompInspectStringExtra")]
    class Patch_RotCompInspectStringExtra_Postfix
    {
        public static void Postfix(ref string __result, CompRottable __instance)
        {
            if (!ThermodynamicsSettings.warmersSlowRot)
                return;

            ThingWithComps rotter = __instance.parent;
            Map map = rotter.Map;
            if (map != null)
            {
                List<Thing> thingList = map.thingGrid.ThingsListAt(rotter.PositionHeld);
                if (thingList != null)
                {
                    foreach (Thing thing in thingList)
                    {
                        if (thing is Building_HeatedStorage bhs && bhs.IsActive())
                        {
                            __result = __result.Replace("Not refrigerated", "Warmed");
                            break;
                        }
                    }
                }
            }
        }
    }
}
