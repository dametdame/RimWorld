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

    
namespace DActiveCircuits.Core_Patches
{

    [HarmonyPatch(typeof(MainTabWindow_Architect))]
    [HarmonyPatch("ClickedCategory")]
    class Patch_ClickedCategory_Postfix
    {
        public static void Postfix()
        {
            ActiveCircuitsBase.UpdateAllNets(Find.CurrentMap);
        }
    }
}
