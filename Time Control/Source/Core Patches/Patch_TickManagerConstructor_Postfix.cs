using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using HarmonyLib;
using Verse;
using System.Reflection;

namespace DTimeControl.Core_Patches
{ 
    [HarmonyPatch(typeof(TickManager))]
    [HarmonyPatch(MethodType.Constructor)]
    class Patch_TickManagerConstructor_Postfix
    {
        public static void Postfix(TickManager __instance)
        {
            FieldInfo tln = AccessTools.Field(typeof(TickManager), "tickListNormal");
            TickList tickListNormal = tln.GetValue(__instance) as TickList;
            FieldInfo tlr = AccessTools.Field(typeof(TickManager), "tickListRare");
            TickList tickListRare = tlr.GetValue(__instance) as TickList;
            FieldInfo tll = AccessTools.Field(typeof(TickManager), "tickListLong");
            TickList tickListLong = tll.GetValue(__instance) as TickList;

            tln.SetValue(__instance, new TCTickList(tickListNormal));
            tlr.SetValue(__instance, new TCTickList(tickListRare));
            tll.SetValue(__instance, new TCTickList(tickListLong));
        }

    }
}
