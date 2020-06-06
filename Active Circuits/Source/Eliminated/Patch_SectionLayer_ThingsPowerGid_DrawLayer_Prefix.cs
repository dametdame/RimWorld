using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
/*
namespace DActiveCircuits
{
    [HarmonyPatch(typeof(SectionLayer_ThingsPowerGrid))]
    [HarmonyPatch("DrawLayer")]
    class Patch_SectionLayer_ThingsPowerGid_DrawLayer_Prefix
    {
        public static bool Prefix(SectionLayer_ThingsPowerGrid __instance)
        {
			if (!OverlayDrawHandler.ShouldDrawPowerGrid || !DebugViewSettings.drawThingsPrinted)
			{
				return true;
			}

			if (!__instance.Visible)
			{
				return false;
			}
			int count = __instance.subMeshes.Count;
			for (int i = 0; i < count; i++)
			{
				LayerSubMesh layerSubMesh = __instance.subMeshes[i];
				if (layerSubMesh.finalized && !layerSubMesh.disabled)
				{
					if (Find.TickManager.TicksGame % 248 == 0) 
					{
						PowerMaterial mat = layerSubMesh.material as PowerMaterial;
						if (mat != null)
							mat.color = ActiveCircuitsBase.GetNetColor(mat.comp);
					}
					//mat.color = Color.white;
					Graphics.DrawMesh(layerSubMesh.mesh, Vector3.zero, Quaternion.identity, layerSubMesh.material, 0);
				}
			}
			
			return false;
		}
    }
}
*/