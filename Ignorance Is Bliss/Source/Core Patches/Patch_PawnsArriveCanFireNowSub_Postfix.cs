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
    [HarmonyPatch(typeof(IncidentWorker_PawnsArrive))]
    [HarmonyPatch("CanFireNowSub")]
    class Patch_PawnsArriveCanFireNowSub_Postfix
    {
        public static void Postfix(ref IncidentParms parms)
        {
            if (parms.faction != null)
            {
                if (!IgnoranceBase.FactionInEligibleTechRange(parms.faction))
                {
                    Faction newFaction = IgnoranceBase.GetRandomEligibleFaction();
                    if (newFaction != null) 
                    {
                        parms.faction = newFaction;
                        return;
                    }
                    return;
                }
                return;
            }
        }
    }
}
