using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetRawData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetRawData), "GetModPlane")]
        public static bool GetModPlane(int index, ref PlanetRawData __instance, ref short __result)
        {
            float baseHeight = 20;

            baseHeight += __instance.GetFactoredScale() * 200 * 100;

            __result = (short)(((__instance.modData[index >> 1] >> (((index & 1) << 2) + 2)) & 3) * 133 + baseHeight);
            return false;
        }
    }
}