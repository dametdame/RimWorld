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
using BetterLoading;

namespace DTechprinting
{
    [StaticConstructorOnStartup]
    public static class PatchBetterLoadingBase
    {

        public static Type blm;

        static PatchBetterLoadingBase()
        {
            /*
            try
            {
                ((Action)(() =>
                {
                    MethodInfo target1;
                    var harmony = new Harmony("io.github.dametri.techprinting");
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.ToLower() == "betterloading"))
                    {
                        Log.Message("DTechprinting: Better Loading running, attempting to patch");

                        blm = AccessTools.TypeByName("BetterLoading.StageRunPostFinalizeCallbacks");

                        target1 = AccessTools.Method(blm, "PreExecToExecWhenFinished");
                        var invoke1 = AccessTools.Method(typeof(Patch_PreExecToExecWhenFinished_Postfix), "Postfix");
                        if (target1 != null && invoke1 != null)
                            harmony.Patch(target1, postfix: new HarmonyMethod(invoke1));
                    }

                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
            */
        }

    }
}
