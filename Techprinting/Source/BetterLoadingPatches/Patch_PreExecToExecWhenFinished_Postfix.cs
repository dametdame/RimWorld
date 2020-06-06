using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;
using BetterLoading;

namespace DTechprinting
{

    class Patch_PreExecToExecWhenFinished_Postfix
    {
        private static void Postfix()
        {
            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.ToLower() == "betterloading"))
            {
                /*
                Log.Message("aarrrr");
                TechprintingBase.Initialize();
                TechprintingBase.MakeThingDictionaries();
                TechprintingBase.SetTechshardPrices();*/
            }
        }
    }

}

