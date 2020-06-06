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

namespace DActiveCircuits
{
    [HarmonyPatch(typeof(Graphic_Linked))]
    [HarmonyPatch("LinkedDrawMatFrom")]
    class Patch_LinkedDrawMatFrom_Postfix
    {

        public static void Postfix(Thing parent, ref Material __result)
        {
            CompPower comp = parent.TryGetComp<CompPower>();
            if (comp != null) 
            {
                __result = new PowerMaterial(__result, comp);
            }
           
        }
    }
}
