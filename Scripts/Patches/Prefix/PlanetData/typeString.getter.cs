using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnPlanetData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetData), nameof(PlanetData.typeString), MethodType.Getter)]
        public static bool typeString(ref string __result, PlanetData __instance)
        {
            __result = "未知".Translate();
            var themeProto = LDB.themes.Select(__instance.theme);
            if (themeProto != null) __result = themeProto.displayName;
            return false;
        }
    }
}