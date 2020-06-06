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

namespace DArcaneTechnology.AwesomeInventoryPatches
{
    [StaticConstructorOnStartup]
    public static class PatchAwesomeInventoryBase
    {

        public static Type aou;

        static PatchAwesomeInventoryBase()
        {

            try
            {
                ((Action)(() =>
                {
                    MethodInfo target1;
                    var harmony = new Harmony("io.github.dametri.arcanetechnology");
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.ToLower() == "awesome inventory"))
                    {
                        Log.Message("D Arcane Technology: Awesome Inventory running, attempting to patch");

                        aou = AccessTools.TypeByName("AwesomeInventory.ApparelOptionUtility");
                        target1 = AccessTools.Method(aou, "CanWear");
                        var invoke1 = AccessTools.Method(typeof(Patch_CanWear_Postfix), "Postfix");
                        if (target1 != null && invoke1 != null)
                            harmony.Patch(target1, postfix: new HarmonyMethod(invoke1));
                    }
                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
        }
    }
}
