using System.Collections;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect 
    {


        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "OnResourceMultiplierValueChange")]
        public static void OnResourceMultiplierValueChange(UIGalaxySelect __instance,
            float val)
        {
            GS2.Config.SetResourceMultiplier(__instance.gameDesc.resourceMultiplier);
        }

  
    }
}