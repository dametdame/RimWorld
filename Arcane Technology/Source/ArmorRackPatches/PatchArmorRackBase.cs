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
using DArcaneTechnology.ArmorRackPatches;

namespace DArcaneTechnology
{
    [StaticConstructorOnStartup]
    public static class PatchArmorRackBase
    {

        public static Type jgul;
        public static Type ar;

        static PatchArmorRackBase()
        {

            try
            {
                ((Action)(() =>
                {
                   
                    var harmony = new Harmony("io.github.dametri.arcanetechnology");
                    if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.ToLower() == "armor racks"))
                    {
                        Log.Message("D Arcane Technology: Armor Racks running, attempting to patch");


                        jgul = AccessTools.TypeByName("ArmorRacks.Jobs.JobDriver_BaseRackJob");
                        MethodInfo target1 = AccessTools.Method(jgul, "TryMakePreToilReservations");
                        var invoke1 = AccessTools.Method(typeof(Patch_BaseRackJob_Prefix), "Prefix");
                        if (target1 != null && invoke1 != null)
                            harmony.Patch(target1, prefix: new HarmonyMethod(invoke1));

                        ar = AccessTools.TypeByName("ArmorRacks.Things.ArmorRack");
                        MethodInfo target2 = AccessTools.Method(ar, "CanStoreWeapon");
                        var invoke2 = AccessTools.Method(typeof(Patch_CanStoreWeapon_Postfix), "Postfix");
                        if (target2 != null && invoke2 != null)
                            harmony.Patch(target2, postfix: new HarmonyMethod(invoke2));

                        MethodInfo target3 = AccessTools.Method(ar, "CanStoreApparel");
                        var invoke3 = AccessTools.Method(typeof(Patch_CanStoreApparel_Postfix), "Postfix");
                        if (target3 != null && invoke3 != null)
                            harmony.Patch(target3, postfix: new HarmonyMethod(invoke3));
                    }
                }))();
            }
            catch (TypeLoadException ex) { Log.Message(ex.ToString()); }
        }
    }
}
