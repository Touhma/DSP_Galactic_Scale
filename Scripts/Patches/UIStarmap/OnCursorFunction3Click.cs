using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnUIStarmap
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarmap), "OnCursorFunction3Click")]
        public static bool OnCursorFunction3Click(UIStarmap __instance)
        {
            return GS2.TP.NavArrowClick(__instance);
        }
    }
}