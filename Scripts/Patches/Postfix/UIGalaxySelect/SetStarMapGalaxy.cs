using HarmonyLib;
using NebulaCompatibility;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), nameof(UIGalaxySelect.SetStarmapGalaxy))]
        public static void SetStarmapGalaxy_Postfix(UIGalaxySelect __instance)
        {
            // this is needed at least in the nebula lobby, throws index out of bounds if not done.
            if (GS3.galaxy == null)
            {
                GS3.Log("Galaxy Null, Returning from Postfix");
                return;
            }
            
            var starmap = __instance.starmap;
            // Increase Pool Count to prevent Nebula from failing to initialize system view when starcount < planetcount
            while (starmap.starPool.Count <= 100)
            {
                var starNode2 = new UIVirtualStarmap.StarNode();
                starNode2.active = false;
                starNode2.starData = null;
                starNode2.pointRenderer = Object.Instantiate(starmap.starPointPrefab, starmap.starPointPrefab.transform.parent);
                starNode2.nameText = Object.Instantiate(starmap.nameTextPrefab, starmap.nameTextPrefab.transform.parent);
                starmap.starPool.Add(starNode2);
            }

            while (starmap.connPool.Count <= 100)
                starmap.connPool.Add(new UIVirtualStarmap.ConnNode
                {
                    active = false,
                    starA = null,
                    starB = null,
                    lineRenderer = Object.Instantiate(starmap.connLinePrefab, starmap.connLinePrefab.transform.parent)
                });
            // End 

            // GS3.Warn("End");
        }
    }
}