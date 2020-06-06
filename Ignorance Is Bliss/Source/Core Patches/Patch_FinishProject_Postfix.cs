
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
namespace DIgnoranceIsBliss.Core_Patches
{
    [HarmonyPatch(typeof(ResearchManager))]
    [HarmonyPatch("FinishProject")]
    class Patch_FinishProject_Postfix
    {
        public static void Postfix()
        {
            IgnoranceBase.playerTechLevel = IgnoranceBase.GetPlayerTech();
        }
    }
}
