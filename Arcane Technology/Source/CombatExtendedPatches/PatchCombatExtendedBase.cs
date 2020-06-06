using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace DArcaneTechnology
{
    [StaticConstructorOnStartup]
    public static class PatchCombatExtendedBase
    {

        public static Type jgul;

        static PatchCombatExtendedBase()
        {

            try
            {
                ((Action)(() =>
                {
                    MethodInfo target1;
                    var harmony = new Harmony("io.github.dametri.arcanetechnology");
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.ToLower() == "combat extended"))
                    {
                        Log.Message("D Arcane Technology: Combat Extended running, attempting to patch");

                        jgul = AccessTools.TypeByName("CombatExtended.JobGiver_UpdateLoadout");
                        target1 = AccessTools.Method(jgul, "AllowedByBiocode");
                        var invoke1 = AccessTools.Method(typeof(Patch_AllowedByBiocode_Postfix), "Postfix");
                        if (target1 != null && invoke1 != null)
                            harmony.Patch(target1, postfix: new HarmonyMethod(invoke1));
                    }
                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
        }
    }
}
