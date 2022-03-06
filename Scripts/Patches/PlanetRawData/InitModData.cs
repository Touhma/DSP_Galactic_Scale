using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetRawData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetRawData), "InitModData")]
        public static bool InitModData(byte[] refModData, ref PlanetRawData __instance, ref byte[] __result)
        {
            __instance.modData = refModData == null ? new byte[__instance.dataLength / 2] : refModData; // changed from .dataLength/2, fixes issue where array can't fit all the data. Shad0wlife is going to take a look and see why it's trying to, but this works for now -innominata
            __result = __instance.modData;
            return false;
        }
    }
}