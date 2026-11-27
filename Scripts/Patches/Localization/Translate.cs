using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnLocalization
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Localization), nameof(Localization.Translate), typeof(string))]
        public static void Translate(string s, ref string __result)
        {
            if (string.IsNullOrEmpty(s) || __result != s) return;
            __result = GSLocalization.Translate(s);
        }
    }
}
