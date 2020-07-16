using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;


namespace DIgnoranceIsBliss.Core_Patches
{
    [StaticConstructorOnStartup]
    public static class IgnoranceBase
    {

        public static Dictionary<Type, TechLevel> incidentWorkers = new Dictionary<Type, TechLevel>
        {
            {typeof(IncidentWorker_MechCluster), TechLevel.Ultra },
            {typeof(IncidentWorker_CrashedShipPart), TechLevel.Ultra },
            {typeof(IncidentWorker_Infestation), TechLevel.Animal },
            {typeof(IncidentWorker_DeepDrillInfestation), TechLevel.Animal },
        };

        public static Dictionary<string, TechLevel> questScriptDefs = new Dictionary<string, TechLevel>()
        {
            { "ThreatReward_MechPods_MiscReward", TechLevel.Ultra}
        };

        public static Dictionary<string, TechLevel> incidentDefNames = new Dictionary<string, TechLevel>
        {
            {"CrystalloidShipPartCrash", TechLevel.Ultra },                  // Crystalloids (Rewrite)
            {"RatkinTunnel_Guerrilla", TechLevel.Industrial },               // NewRatkinPlus
            {"RatkinTunnel_Thief", TechLevel.Industrial },                   // NewRatkinPlus
            {"VFEM_BlackKnight", TechLevel.Medieval },                       // Vanilla Factions Expanded - Medieval
            {"PsychicEmitterActivationSW", TechLevel.Spacer },               // Sparkling Worlds
            {"WeaponsCachePodCrashSW", TechLevel.Spacer },                   // Sparkling Worlds
            { "AA_Incident_BlackHive", TechLevel.Spacer },                   // Alpha animals, they're technically animals but OP as hell
        };

        public static Dictionary<TechLevel, List<ResearchProjectDef>> strataDic = new Dictionary<TechLevel, List<ResearchProjectDef>>();

        public static TechLevel playerTechLevel
        {
            get
            {
                if (cachedTechLevel == TechLevel.Undefined)
                {
                    cachedTechLevel = GetPlayerTech();
                }
                return cachedTechLevel;
            }
            set
            {
                bool flag = false;
                if (cachedTechLevel == TechLevel.Undefined)
                {
                    flag = true;
                }
                cachedTechLevel = value;
                if (flag)
                {

                }
            }
        }
        private static TechLevel cachedTechLevel = TechLevel.Undefined;

        

        static IgnoranceBase()
        {
            foreach(ResearchProjectDef rpd in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
            {
                if (!strataDic.ContainsKey(rpd.techLevel))
                {
                    strataDic.Add(rpd.techLevel, new List<ResearchProjectDef>());
                }
                strataDic[rpd.techLevel].Add(rpd);
            }
        }

        public static void WarnIfNoFactions()
        {
            if (NumFactionsInRange() <= 0)
            {
                Messages.Message("DNoValidFactions".Translate(), null, MessageTypeDefOf.NegativeEvent, false);
            }
        }

        public static TechLevel GetPlayerTech()
        {
            if (IgnoranceSettings.useHighestResearched)
            {
                for (int i = (int)TechLevel.Archotech; i > 0; i--)
                {
                    if (!strataDic.ContainsKey((TechLevel)i))
                        continue;
                    foreach (ResearchProjectDef rpd in strataDic[(TechLevel)i])
                    {
                        if (rpd.IsFinished)
                            return (TechLevel)i;
                    }
                }
                return TechLevel.Animal;
            }
            else if (IgnoranceSettings.usePercentResearched)
            {
                int numResearched = 0;
                for (int i = (int)TechLevel.Archotech; i > 0; i--)
                {
                    if (!strataDic.ContainsKey((TechLevel)i))
                        continue; 
                    foreach (ResearchProjectDef rpd in strataDic[(TechLevel)i])
                    {
                        if (rpd.IsFinished)
                            numResearched++;
                    }
                    if ((float)numResearched / (float)strataDic[(TechLevel)i].Count >= IgnoranceSettings.percentResearchNeeded)
                        return (TechLevel)i;
                }
                return TechLevel.Animal;
            }
            else // (IgnoranceSettings.useActualTechLevel) or fixedTechRange
            {
                return Faction.OfPlayer.def.techLevel;
            }
        }

        public static IEnumerable<Faction> HostileFactions()
        {
            return from f in Find.FactionManager.AllFactions
                   where !f.IsPlayer && !f.defeated && f.HostileTo(Faction.OfPlayer) && f.def.pawnGroupMakers != null && f.def.pawnGroupMakers.Any((PawnGroupMaker x) => x.kindDef == PawnGroupKindDefOf.Combat)
                   select f;
        }

        public static Faction GetRandomEligibleFaction()
        {
            var hostile = HostileFactions().Where(f => TechIsEligibleForIncident(f.def.techLevel));
            if (hostile != null && hostile.Count() > 0)
                return hostile.RandomElement();
            return null;
        }

        public static int NumFactionsInRange()
        {
            return HostileFactions().Where(f => TechIsEligibleForIncident(f.def.techLevel)).Count();
        }

        public static int NumFactionsBelow(TechLevel tech)
        {
            return HostileFactions().Where(f => f.def.techLevel < tech && TechIsEligibleForIncident(f.def.techLevel)).Count();
        }

        public static int NumFactionsAbove(TechLevel tech)
        {
            return HostileFactions().Where(f => f.def.techLevel > tech && TechIsEligibleForIncident(f.def.techLevel)).Count();
        }

        public static int NumFactionsEqual(TechLevel tech)
        {
            return HostileFactions().Where(f => f.def.techLevel == tech && TechIsEligibleForIncident(f.def.techLevel)).Count();
        }

       

        public static bool TechIsEligibleForIncident(TechLevel tech)
        {
            if (tech == TechLevel.Undefined)
                return true;
            if (IgnoranceSettings.useFixedTechRange)
            {
                return ((int)tech >= IgnoranceSettings.fixedRange.min && (int)tech <= IgnoranceSettings.fixedRange.max);
            }
            int playerTech = (int)playerTechLevel;
            int techInt = (int)tech;
            if (playerTech < techInt)
            {
                if (IgnoranceSettings.numTechsAhead >= 0)
                    return (playerTech + IgnoranceSettings.numTechsAhead >= techInt);
            }
            else if (playerTech > techInt)
            {
                if (IgnoranceSettings.numTechsBehind >= 0)
                    return (playerTech - IgnoranceSettings.numTechsBehind <= techInt);
            }
            return true;
        }

        public static bool FactionInEligibleTechRange(Faction f)
        {
            return TechIsEligibleForIncident(f.def.techLevel);
        }

    }
}
