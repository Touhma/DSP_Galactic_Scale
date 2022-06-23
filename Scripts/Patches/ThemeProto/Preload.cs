using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnThemeProto
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ThemeProto), "Preload")]
        public static bool Preload(ref ThemeProto __instance)
        {
            __instance.displayName = __instance.DisplayName.Translate();
            __instance.terrainMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "terrain", "{0}-{1}", true);
            __instance.oceanMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "ocean", "{0}-{1}", true);
            __instance.atmosMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "atmosphere", "{0}-{1}", true);
            __instance.lowMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "low", "{0}-{1}", true);
            __instance.thumbMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "thumb", "{0}-{1}", true);
            __instance.minimapMat = Utils.ResourcesLoadArray<Material>(__instance.MaterialPath + "minimap", "{0}-{1}", true);
            __instance.ambientDesc = Utils.ResourcesLoadArray<AmbientDesc>(__instance.MaterialPath + "ambient", "{0}-{1}", true);
            __instance.ambientSfx = Utils.ResourcesLoadArray<AudioClip>(__instance.SFXPath, "{0}-{1}", true);
            if (__instance.RareSettings.Length != __instance.RareVeins.Length * 4) Debug.LogError("稀有矿物数组长度有误 " + __instance.displayName);
            return false;
        }
    }
}