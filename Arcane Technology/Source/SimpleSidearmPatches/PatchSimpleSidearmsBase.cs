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

namespace DArcaneTechnology.SimpleSidearmPatches
{
    [StaticConstructorOnStartup]
    public static class PatchSimpleSidearmsBase
    {

        public static Type aou;

        static PatchSimpleSidearmsBase()
        {

            try
            {
                ((Action)(() =>
                {
                    MethodInfo target1;
                    var harmony = new Harmony("io.github.dametri.arcanetechnology");
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.ToLower() == "simple sidearms"))
                    {
                        Log.Message("D Arcane Technology: Simple sidearms running, attempting to patch");
                        aou = AccessTools.TypeByName("SimpleSidearms.utilities.StatCalculator");
                        target1 = AccessTools.Method(aou, "isValidSidearm");
                        var invoke1 = AccessTools.Method(typeof(Patch_isValidSidearm_Postfix), "Postfix");
                        if (target1 != null && invoke1 != null)
                            harmony.Patch(target1, postfix: new HarmonyMethod(invoke1));
                    }
                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
        }
    }
}
