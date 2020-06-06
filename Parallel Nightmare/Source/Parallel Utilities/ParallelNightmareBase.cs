using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;
using HarmonyLib;

namespace DTimeControl.Parallel_Nightmare
{
    [StaticConstructorOnStartup]
    public static class ParallelNightmareBase
    {
        static ParallelNightmareBase()
        {
            try
            {
                ((Action)(() =>
                {/*
                     var harmony = new Harmony("io.github.dametri.parallelnightmare");

                    IEnumerable<Type> poolTypes = TypeUtility.AllGenericTypes(typeof(SimplePool<>));

                    var invokePoolPrefix = AccessTools.Method(typeof(Patch_SimplePoolGet_PrefixPostfix), "Prefix");
                    var invokePoolPostfix = AccessTools.Method(typeof(Patch_SimplePoolGet_PrefixPostfix), "Postfix");

                    foreach (Type t in poolTypes)
                    {
                        var target1 = AccessTools.Method(t, "Get");
                        harmony.Patch(target1, prefix: new HarmonyMethod(invokePoolPrefix), postfix: new HarmonyMethod(invokePoolPostfix));
                        harmony.Patch(target1, postfix: new HarmonyMethod(invokePoolPrefix));
                    }
                    */
                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
        }
    }
}
