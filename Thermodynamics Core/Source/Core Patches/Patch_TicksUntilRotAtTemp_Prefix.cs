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
    [HarmonyPatch("TicksUntilRotAtTemp")]
    class Patch_TicksUntilRotAtTemp_Prefix
    {
        public static bool Prefix(ref float temp, CompRottable __instance)
        {
            if (!ThermodynamicsSettings.warmersSlowRot)
                return true;

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
                            temp = 1f;
                            break;
                        }
                    }
                }
            }        
            return true;
        }
    }
}
