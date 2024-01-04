﻿using HarmonyLib;

namespace GalacticScale.Patches
{
    //The Patches in this class Add a PreLoad Splash
    public partial class PatchOnVFPreload
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(VFPreload), nameof(VFPreload.Update))]
        public static void Update(VFPreload __instance)
        {
            __instance.splashes[0].gameObject.SetActive(true);
            __instance.splashes[1].gameObject.SetActive(false);
        }
    }
}