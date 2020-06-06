using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Noise;

namespace DHighVoltage
{
    [StaticConstructorOnStartup]
    public class HighVoltageBase
    {


        public static Dictionary<string, float> capacityMults = new Dictionary<string, float>
        {
            { ThingDefOf.Steel.defName, 1f },
            { ThingDefOf.Silver.defName,  1f},
            { ThingDefOf.Gold.defName, 2f },
            { ThingDefOf.Uranium.defName, 1.2f },
            { ThingDefOf.Plasteel.defName, 0.7f },
             
            { "NECIron", 1.15f },               // Vanilla Iron and Steel
            { "Owl_Iron", 1.15f },              // Simple Chains: Steel
            { "Iron", 1.15f },                  // Metals+
            { "Copper", 1.80f },                // Metals+
            { "Tin", 0.9f },                    // Metals+
            { "Bronze", 1.3f },                 // Metals+
            { "Titanium", 1f },                 // Metals+ & Glittertech
            { "FiberComposite", 0.6f },         // Rimefeller
            { "AA_SkySteel",  0.7f},            // Alpha Animals
            { "Crysteel",  0.25f},              // Crystalloid (Rewrite)
            { "PTAthenium", 1.1f},              // PsiTech  
            { "AlphaPoly", 2.5f},               // Glittertech
            { "BetaPoly", 3.5f},                // Glitterech
            { "Ceramite", 0.7f},                // Imperial Guard Core Mod 
            { "Adamantium", 4f },               // Imperial Guard Core Mod
            { "CAL_Copper", 1.8f},              // Cupro's Alloys (Continued)
            { "CAL_Lead", 1.1f},                // Cupro's Alloys (Continued)
            { "CAL_Tin", 0.9f},                 // Cupro's Alloys (Continued)
            { "CAL_Aluminum", 2.0f},            // Cupro's Alloys (Continued)
            { "CAL_Nickel", 1.25f},             // Cupro's Alloys (Continued)
            { "CAL_Zinc", 1.2f},                // Cupro's Alloys (Continued)
            { "CAL_Bismuth", 1f},               // Cupro's Alloys (Continued)
            { "CAL_Chromium", 0.7f},            // Cupro's Alloys (Continued)
            { "CAL_Plumchalcum", 1.5f},         // Cupro's Alloys (Continued)
            { "CAL_Duralumin", 2.0f},           // Cupro's Alloys (Continued)
            { "CAL_Zamak", 1.6f},               // Cupro's Alloys (Continued)
            { "CAL_BismuthBronze", 1.3f},       // Cupro's Alloys (Continued)
            { "CAL_Bronze", 1.3f},              // Cupro's Alloys (Continued)
            { "CAL_Cupronickel", 2.1f},         // Cupro's Alloys (Continued)
            { "CAL_StainlessSteel", 0.7f},      // Cupro's Alloys (Continued)
            { "CAL_Hepatzion", 1.95f},          // Cupro's Alloys (Continued)
            { "CAL_Brass", 0.8f},               // Cupro's Alloys (Continued)
            { "CAL_ChromePlatedSteel", 0.7f},   // Cupro's Alloys (Continued)
            { "CAL_Aurichalcum", 2.2f},         // Cupro's Alloys (Continued)
            { "CAL_SterlingSilver", 1.8f},      // Cupro's Alloys (Continued)
            { "CAL_RoseGold", 1.9f},            // Cupro's Alloys (Continued)
            { "CAL_Electrum", 2f},              // Cupro's Alloys (Continued)
            { "CAL_BlackSteel", 2.5f},          // Cupro's Alloys (Continued)
            { "CAL_Plastin", 0.4f},             // Cupro's Alloys (Continued)

        };

        public static Dictionary<string, float> outputMults = new Dictionary<string, float>
        {
            {ThingDefOf.Steel.defName, 0.9f },
            {ThingDefOf.Silver.defName, 1.5f },
            {ThingDefOf.Gold.defName, 2f },
            {ThingDefOf.Uranium.defName, 0.8f },
            {ThingDefOf.Plasteel.defName, 0.5f },

            { "NECIron", 1f },
            { "Owl_Iron", 1f },
            { "Iron", 1f },
            { "Copper", 1.3f },
            { "Tin", 1f },
            { "Bronze", 1.1f },
            { "Titanium", 0.75f },
            { "FiberComposite", 0.5f },
            { "AA_SkySteel",  0.75f},
            { "Crysteel",  0.45f},
            { "PTAthenium", 2.75f},
            { "AlphaPoly", 3f},
            { "BetaPoly", 4f},
            { "Ceramite", 0.4f},
            { "Adamantium", 1f },
            { "CAL_Copper", 1.3f},
            { "CAL_Lead", 0.8f},
            { "CAL_Tin", 1f},
            { "CAL_Aluminum", 1.1f},
            { "CAL_Nickel", 1f},
            { "CAL_Zinc", 1.2f},
            { "CAL_Bismuth", 1.1f},
            { "CAL_Chromium", 0.8f},
            { "CAL_Plumchalcum", 1.4f},
            { "CAL_Duralumin", 1.5f},
            { "CAL_Zamak", 1.15f},
            { "CAL_BismuthBronze", 1.15f},
            { "CAL_Bronze", 1.1f},
            { "CAL_Cupronickel", 0.9f},
            { "CAL_StainlessSteel", 0.75f},
            { "CAL_Hepatzion", 2.1f},
            { "CAL_Brass", 1.75f },
            { "CAL_ChromePlatedSteel", 0.6f},
            { "CAL_Aurichalcum", 1.4f},
            { "CAL_SterlingSilver", 1.65f},
            { "CAL_RoseGold", 1.65f},
            { "CAL_Electrum", 2f},
            { "CAL_BlackSteel", 1.8f},
            { "CAL_Plastin", 0.5f},
        };

        public static float GetCapacityMult(ThingDef stuff)
        {
            if (stuff == null)
                return 0.75f;
            float dictVal;
            if (capacityMults.TryGetValue(stuff.defName, out dictVal))
                return dictVal;
            List<StatModifier> sp = stuff.stuffProps.statFactors;
            if (sp != null)
            {
                StatModifier mhp = sp.Find(x => x.stat == StatDefOf.MaxHitPoints);
                if (mhp != null)
                {
                    return 1f / mhp.value * 0.75f;
                }
            }
            return 0.75f;
        }

        public static float GetOutputMult(ThingDef stuff)
        {
            float dictVal;
            if (outputMults.TryGetValue(stuff.defName, out dictVal))
                return dictVal;
            return 1f;
        }

        static HighVoltageBase()
        {
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (def.GetCompProperties<CompProperties_DVoltage>() == null)
                {
                    CompProperties_Power powerComp = def.GetCompProperties<CompProperties_Power>();
                    if (powerComp != null)
                    {
                        if (powerComp.compClass == typeof(CompPowerTransmitter))
                        {
                            if (def.stuffCategories == null)
                            {
                                if (def.costList != null)
                                {
                                    StuffIt(def);
                                }
                                def.ResolveReferences();
                                def.PostLoad();
                            }
                            CompProperties_DVoltage comp = new CompProperties_DVoltage();
                            def.comps.Add(comp);
                        }

                    }
                }
            }
            CostListCalculator.Reset();
            GenConstruct.Reset();
            DefDatabase<DesignationCategoryDef>.ResolveAllReferences();
        }

        public static void StuffIt(ThingDef def)
        {
            int costStuffCount = 0;
            foreach (ThingDefCountClass count in def.costList)
            {
                if (count.thingDef == ThingDefOf.Steel)
                {
                    costStuffCount += count.count;
                }
            }
            if (costStuffCount > 0)
            {
                def.costList.RemoveAll(x => x.thingDef == ThingDefOf.Steel);
                def.stuffCategories = new List<StuffCategoryDef>();
                def.stuffCategories.Add(StuffCategoryDefOf.Metallic);
                def.costStuffCount = costStuffCount;
                ThingDef frameDef = DefDatabase<ThingDef>.GetNamed(ThingDefGenerator_Buildings.BuildingFrameDefNamePrefix + def.defName);
                frameDef.stuffCategories = def.stuffCategories;
            }
        }


    }
}
